using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;

namespace Pillbox.controls.Forms
{
    public partial class CameraCapture : Form
    {
        private VideoCapture _capture;
        private Mat _frame;
        private System.Windows.Forms.Timer _timer;
        private TesseractEngine _ocrEngine;
        private readonly BitmapToPixConverter _pixConverter = new BitmapToPixConverter();
        private bool _useFrontMirror = true;

        // OCR stability settings
        private const float _confidenceThreshold = 0.65f;
        private const int StableThreshold = 5;
        private const double SimilarityThreshold = 0.60;
        private string _previousText = string.Empty;
        private int _stableCount = 0;

        // how many frames to grab per “burst”
        private const int BurstCount = 5;

        // on-screen instruction label
        private Label lblInstruction;

        public string outputString { get; private set; }

        public CameraCapture()
        {
            InitializeComponent();
            InitializeCamera();
            SetupInstructionLabel();  // add instruction label under video feed
        }

        private void SetupInstructionLabel()
        {
            lblInstruction = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 30,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblInstruction);
        }

        private void InitializeCamera()
        {
            _capture = new VideoCapture(0);
            if (!_capture.IsOpened())
            {
                MessageBox.Show("Could not open the camera.");
                return;
            }

            _capture.Set(VideoCaptureProperties.FrameWidth, 2048);
            _capture.Set(VideoCaptureProperties.FrameHeight, 1152);
            Debug.WriteLine(
                $"Camera resolution: {_capture.Get(VideoCaptureProperties.FrameWidth)}×" +
                $"{_capture.Get(VideoCaptureProperties.FrameHeight)}"
            );

            _frame = new Mat();
            _ocrEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

            _timer = new System.Windows.Forms.Timer { Interval = 10 }; // ~20 FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop(); // prevent re-entry
            try
            {
                var raw = CaptureBurstAndSelectSharp(BurstCount);
                if (raw == null) return;
                if (_useFrontMirror)
                    Cv2.Flip(raw, raw, FlipMode.Y);

                // detect enclosure
                using var gray = new Mat();
                Cv2.CvtColor(raw, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);
                using var edges = new Mat();
                Cv2.Canny(gray, edges, 50, 150);
                var contours = Cv2.FindContoursAsArray(edges, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                var largest = contours.OrderByDescending(c => Cv2.ContourArea(c)).FirstOrDefault();

                // default: no instruction
                lblInstruction.Text = string.Empty;

                if (largest != null)
                {
                    var peri = Cv2.ArcLength(largest, true);
                    var approx = Cv2.ApproxPolyDP(largest, 0.02 * peri, true);
                    if (approx.Length == 4)
                    {
                        var rect = OrderQuad(approx);
                        // coverage
                        double quadArea = Cv2.ContourArea(approx);
                        double frameArea = raw.Width * raw.Height;
                        double coverage = quadArea / frameArea;
                        // skew angle
                        var top = rect[1] - rect[0];
                        double angleDeg = Math.Abs(Math.Atan2(top.Y, top.X) * 180.0 / Math.PI);
                        // flatness
                        double topLen = Distance(rect[1], rect[0]);
                        double bottomLen = Distance(rect[2], rect[3]);
                        double ratioWB = topLen / bottomLen;

                        if (coverage < 0.2)
                            lblInstruction.Text = "Move closer";
                        else if (coverage > 0.8)
                            lblInstruction.Text = "Move further";
                        else if (angleDeg > 5)
                            lblInstruction.Text = "Keep camera level";
                        else if (Math.Abs(ratioWB - 1) > 0.15)
                            lblInstruction.Text = "Hold document flat";
                    }
                }

                // prepare doc for OCR
                var doc = raw;

                // OCR
                string text;
                float meanConf;
                using (var docGray = new Mat())
                {
                    Cv2.CvtColor(doc, docGray, ColorConversionCodes.BGR2GRAY);
                    using var bmp = BitmapConverter.ToBitmap(docGray);
                    using var pix = _pixConverter.Convert(bmp);
                    using var page = _ocrEngine.Process(pix, PageSegMode.Auto);
                    text = (page.GetText() ?? string.Empty).Trim();
                    meanConf = page.GetMeanConfidence();
                }

                BeginInvoke(new Action(() =>
                {
                    lblConfidence.Text = $"Confidence: {(meanConf * 100):F1}%";
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = BitmapConverter.ToBitmap(doc);
                }));

                // stability
                if (meanConf >= _confidenceThreshold && !string.IsNullOrWhiteSpace(text))
                {
                    double sim = Similarity(text, _previousText);
                    if (sim >= SimilarityThreshold)
                        _stableCount++;
                    else
                    {
                        _previousText = text;
                        _stableCount = 0;
                    }

                    if (_stableCount >= StableThreshold)
                    {
                        outputString = text;
                        MessageBox.Show("Final (stable) OCR result:\n" + Normalize(text));
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                {
                    _stableCount = 0;
                }
            }
            finally
            {
                _timer.Start();
            }
        }

       

        /// <summary>
        /// Grabs a small burst of frames, scores each by Laplacian variance,
        /// and returns the single sharpest Mat.
        /// </summary>
        private Mat CaptureBurstAndSelectSharp(int count)
        {
            var candidates = new List<(Mat frame, double score)>();
            for (int i = 0; i < count; i++)
            {
                var f = new Mat();
                if (!_capture.Read(f) || f.Empty())
                {
                    f.Dispose();
                    continue;
                }

                double s = MeasureSharpness(f);
                candidates.Add((f, s));
            }

            if (candidates.Count == 0)
                return null;

            var best = candidates.OrderByDescending(x => x.score).First().frame;
            foreach (var (frame, _) in candidates)
                if (!ReferenceEquals(frame, best))
                    frame.Dispose();

            return best;
        }

        /// <summary>
        /// Sharpness metric = variance of Laplacian.
        /// </summary>
        private double MeasureSharpness(Mat frame)
        {
            using var gray = new Mat();
            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
            using var lap = new Mat();
            Cv2.Laplacian(gray, lap, MatType.CV_64F);
            Cv2.Pow(lap, 2, lap);
            return Cv2.Mean(lap).Val0;
        }

        /// <summary>
        /// Finds the largest 4‑point contour, orders its corners, and
        /// perspective‑warps it to the full image rectangle.
        /// </summary>
        private Mat DetectAndWarpDocument(Mat src)
        {
            int W = src.Width, H = src.Height;
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);

            using var edges = new Mat();
            Cv2.Canny(gray, edges, 50, 150);

            var contours = Cv2.FindContoursAsArray(
                edges,
                RetrievalModes.List,
                ContourApproximationModes.ApproxSimple
            );
            var largest = contours
                .OrderByDescending(c => Cv2.ContourArea(c))
                .FirstOrDefault();
            if (largest == null)
                return src;

            double peri = Cv2.ArcLength(largest, true);
            var approx = Cv2.ApproxPolyDP(largest, 0.02 * peri, true);
            if (approx.Length != 4)
                return src;

            var rect = OrderQuad(approx);
            var dst = new[]
            {
                new Point2f(0,   0),
                new Point2f(W-1, 0),
                new Point2f(W-1, H-1),
                new Point2f(0,   H-1)
            };

            var M = Cv2.GetPerspectiveTransform(rect, dst);
            var warped = new Mat();
            Cv2.WarpPerspective(src, warped, M, new OpenCvSharp.Size(W, H));
            return warped;
        }

        private Point2f[] OrderQuad(OpenCvSharp.Point[] pts)
        {
            var list = pts.Select(p => new Point2f(p.X, p.Y)).ToList();
            return new[]
            {
                list.OrderBy(p => p.X + p.Y).First(),   // TL
                list.OrderBy(p => p.Y - p.X).First(),   // TR
                list.OrderBy(p => p.X + p.Y).Last(),    // BR
                list.OrderBy(p => p.Y - p.X).Last()     // BL
            };
        }


        /// <summary>
        /// Compute the Levenshtein edit distance between two strings.
        /// </summary>
        private int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;

            var dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1,
                                 dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }

            return dp[a.Length, b.Length];
        }
        /// <summary>
        /// Normalize to lowercase and collapse non-word chars to spaces.
        /// </summary>
        private string Normalize(string s)
        {
            var lower = (s ?? string.Empty).ToLowerInvariant();
            return Regex.Replace(lower, @"\W+", " ").Trim();
        }

        /// <summary>
        /// Compute similarity ratio between 0 and 1 based on Levenshtein.
        /// </summary>
        private double Similarity(string s1, string s2)
        {
            var a = Normalize(s1);
            var b = Normalize(s2);
            int dist = LevenshteinDistance(a, b);
            int max = Math.Max(a.Length, b.Length);
            return max == 0 ? 1.0 : 1.0 - (double)dist / max;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer?.Stop(); _timer?.Dispose();
            _capture?.Release(); _capture?.Dispose();
            _ocrEngine?.Dispose();
            _frame?.Dispose();
            base.OnFormClosed(e);
        }

        private void FrontCameraButton_Click(object sender, EventArgs e)
        {
            _useFrontMirror = !_useFrontMirror;
            FrontCameraButton.Text = _useFrontMirror
                ? "Rear Camera"
                : "Front Camera";
        }

        private double Distance(Point2f a, Point2f b)
            => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
}



