using RoadFlow.Utility.Exceptions;
using RoadFlow.Utility.Files.Paths;
using System.IO;
using System.Threading.Tasks;

namespace RoadFlow.Utility.Files
{
    /// <summary>
    /// 本地文件存储服务
    /// </summary>
    public class DefaultFileStore : IFileStore
    {
        /// <summary>
        /// 路径生成器
        /// </summary>
        private readonly IPathGenerator _generator;

        /// <summary>
        /// 初始化本地文件存储服务
        /// </summary>
        /// <param name="pathGenerator">路径生成器</param>
        public DefaultFileStore(IPathGenerator pathGenerator)
        {
            _generator = pathGenerator;
        }

        /// <summary>
        /// 保存文件,返回完整文件路径 w
        /// </summary>
        public async Task<string> SaveAsync()
        {
            var fileControl = WebHelper.GetFile();
            var path = _generator.Generate(fileControl.FileName);
            var physicalPath = CommonHelper.GetWebRootPath(path);
            var directory = Path.GetDirectoryName(physicalPath);
            if (string.IsNullOrEmpty(directory))
                throw new Warning("上传失败");
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await fileControl.CopyToAsync(stream);
            }
            return path;
        }
    }
}
