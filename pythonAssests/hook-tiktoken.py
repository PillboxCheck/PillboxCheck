from PyInstaller.utils.hooks import get_package_paths
import os

package_name = "tiktoken"
module_path = get_package_paths(package_name)[1]
# This assumes that there is an "encodings" folder inside the tiktoken package
datas = [
    (os.path.join(module_path, "encodings"), os.path.join(package_name, "encodings"))
]
