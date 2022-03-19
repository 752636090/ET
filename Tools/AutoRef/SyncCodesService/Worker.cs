namespace SyncCodesService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _workSpace;
        private FileSystemWatcher _watcher;

        //private bool _refreshed = false;
        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
            if(string.IsNullOrEmpty(Args.WorkPlace))
            {
                _workSpace = config["MyConfig:WorkSpace"];
            }
            else
            {
                _workSpace = Args.WorkPlace;
            }

            string codesRoot = Path.Combine(_workSpace, "Codes");
            _watcher = new FileSystemWatcher(codesRoot);
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_refreshed = false;
                await Task.Delay(1000, stoppingToken);
            }
            _watcher.EnableRaisingEvents = false;
        }


        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Refresh(e.FullPath, false);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Refresh(e.FullPath, true);
        }

        private void Refresh(string path, bool isAdd)
        {
            //if (_refreshed)
            //{
            //    return;
            //}

            if (Path.GetExtension(path).ToLower() != ".cs")
            {
                return;
            }
            string root = _workSpace;
            if (!Directory.Exists(root))
            {
                _logger.LogError($"目录{root}不存在,检查参数");
            }
            if (path.Contains(@"\Model\"))
            {
                AdjustTool.Adjust(Path.Combine(root, "Unity.Model.csproj"), "Model", path, isAdd, false);
            }
            else if (path.Contains(@"\ModelView\"))
            {
                AdjustTool.Adjust(Path.Combine(root, "Unity.ModelView.csproj"), "ModelView", path, isAdd, false);
            }
            else if (path.Contains(@"\Hotfix\"))
            {
                AdjustTool.Adjust(Path.Combine(root, "Unity.Hotfix.csproj"), "Hotfix", path, isAdd, false);
            }
            else if (path.Contains(@"\HotfixView\"))
            {
                AdjustTool.Adjust(Path.Combine(root, "Unity.HotfixView.csproj"), "HotfixView", path, isAdd, false);
            }
            //_refreshed = true;
        }
    }
}