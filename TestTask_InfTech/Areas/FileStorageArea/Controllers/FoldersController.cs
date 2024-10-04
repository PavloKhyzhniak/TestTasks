using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask_InfTech.DB;
using TestTask_InfTech.DB.Model;
using TestTask_InfTech.ViewModels;
using System.IO;
using System.Xml.Serialization;
using static NuGet.Packaging.PackagingConstants;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer;

namespace TestTask_InfTech.Areas.FileStorageArea.Controllers
{
    [Area("FileStorageArea")]
    public class FoldersController : Controller
    {
        private const string csSystemSeparator = "\\";
        private readonly FileStorageDB _context;
        private const string SessionCurrentFolderGuid = "CurrentFolderGuid";
        private const string SessionCurrentElementGuid = "CurrentElementGuid";

        private string projectRootPath;
        private string outputPath;
        private string storagePath;

        public FoldersController(FileStorageDB aContext)
        {
            _context = aContext;

            projectRootPath = AppDomain.CurrentDomain.BaseDirectory;
            outputPath = Path.Combine(projectRootPath, "wwwroot/Content");
            storagePath = Path.Combine(projectRootPath, "storage");
        }

        // GET: FileStorageArea/Folders
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentFolderElement = HttpContext.Session.GetString(SessionCurrentFolderGuid) ?? string.Empty;
            ViewBag.CurrentElement = HttpContext.Session.GetString(SessionCurrentElementGuid) ?? string.Empty;

            //начнем с обработки корневых папок
            var tmpFolders = await _context.Folder.Where(a => a.Parental == null).ToListAsync();
            var tmpSubNodeModels = new List<SubNodeModel>();
            var tmpFoldersGuid = new List<Guid>();
            var tmpFilesGuid = new List<Guid>();
            
            _ = await _context.Ext.ToListAsync();

            await PrepareSubNode(tmpFolders, tmpSubNodeModels, tmpFoldersGuid, tmpFilesGuid);

            return View("IndexTree", tmpSubNodeModels);
        }

        private async Task PrepareSubNode(List<Folder> aFolders, List<SubNodeModel> aSubNodeModels, List<Guid> aFoldersGuid, List<Guid> aFilesGuid)
        {
            foreach (var tmpFolder in aFolders)
            {
                //папка уже обработана
                if (aFoldersGuid.Contains(tmpFolder.FolderId))
                    continue;

                //папку начали обрабатывать
                aFoldersGuid.Add(tmpFolder.FolderId);

                var tmpSubFolders = await _context.Folder.Where(a => (a.Parental != null) && (a.Parental.FolderId.Equals(tmpFolder.FolderId))).ToListAsync();
                List<SubNodeModel> tmpSubNodeModels = new List<SubNodeModel>();

                await PrepareSubNode(tmpSubFolders, tmpSubNodeModels, aFoldersGuid, aFilesGuid);

                //создадим обьект дерева
                var tmpNode = new SubNodeModel()
                {
                    ChildrenFolder = tmpSubNodeModels,
                    CurrentFolder = tmpFolder,
                };
                var tmpChildrenFile = await _context.Files.Where(a => a.Folder.FolderId.Equals(tmpFolder.FolderId) && a.Extension!=null).ToListAsync();
                tmpNode.ChildrenFile = tmpChildrenFile;

                aSubNodeModels.Add(tmpNode);
            }
        }

        // GET: FileStorageArea/Folders/Choice/5
        public IActionResult Choice(Guid? aId)
        {
            //            ViewBag.CurrentFileElement = aId.ToString();
            ViewBag.CurrentElement = aId.ToString();

            HttpContext.Session.SetString(SessionCurrentFolderGuid, aId.ToString() ?? string.Empty);
            ViewBag.CurrentFolderElement = aId.ToString();

            return RedirectToAction("Index");
        }

        // GET: FileStorageArea/Folders/Details/5
        public async Task<IActionResult> Details(Guid? aId)
        {
            if (aId == null)
            {
                return NotFound();
            }

            var tmpFolder = await _context.Folder
                .FirstOrDefaultAsync(m => m.FolderId == aId);
            if (tmpFolder == null)
            {
                return NotFound();
            }

            return View(tmpFolder);
        }

        // GET: FileStorageArea/Folders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileStorageArea/Folders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolderId,Name")] Folder aFolder)
        {
            if (ModelState.IsValid)
            {
                aFolder.FolderId = Guid.NewGuid();
                //подключение родительской папки
                var tmpCurrentFolderId = HttpContext.Session.GetString(SessionCurrentFolderGuid) ?? string.Empty;
                if (!tmpCurrentFolderId.Equals(string.Empty))
                {
                    aFolder.Parental = await _context.Folder.Where(a => a.FolderId.ToString().Equals(tmpCurrentFolderId)).FirstOrDefaultAsync();
                }

                await CreateFolderAsync(aFolder);

                //перевод на вновь созданную папку
                HttpContext.Session.SetString(SessionCurrentFolderGuid, aFolder.FolderId.ToString());
                HttpContext.Session.SetString(SessionCurrentElementGuid, aFolder.FolderId.ToString());

                _context.Folder.Add(aFolder);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(aFolder);
        }

        private async Task<bool> CreateFolderAsync(Folder aFolder)
        {
            var tmpFolderPath = await GetRealFolderPathAsync(aFolder, aFolder.Name);

            string tmpBaseStr = AppDomain.CurrentDomain.BaseDirectory;

            var tmpFullServerPath = string.Format("{1}{0}", tmpFolderPath, tmpBaseStr);
            var tmpDirectory = System.IO.Directory.CreateDirectory(tmpFullServerPath);
            if (tmpDirectory.Exists)
                return true;

            return false;
        }

        private async Task<string> GetRealFolderPathAsync(Folder aFolder, string aPath)
        {
            //путь уже вычислен до корня
            if (aFolder.Parental == null)
                return aPath;

            //получим папку верхнего уровня относительно текущей
            var tmpPath = string.Format("{2}{1}{0}", aPath, csSystemSeparator, aFolder.Parental.Name);
            return await GetRealFolderPathAsync(aFolder.Parental, tmpPath);
        }

        // GET: FileStorageArea/Folders/Edit/5
        public async Task<IActionResult> Edit(Guid? aId)
        {
            if (aId == null)
            {
                return NotFound();
            }

            var tmpFolder = await _context.Folder.FindAsync(aId);
            if (tmpFolder == null)
            {
                return NotFound();
            }
            return View(tmpFolder);
        }

        // POST: FileStorageArea/Folders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid aId, [Bind("FolderId,Name")] Folder aFolder)
        {
            if (aId != aFolder.FolderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Folder.Update(aFolder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FolderExists(aFolder.FolderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aFolder);
        }

        // GET: FileStorageArea/Folders/Delete/5
        public async Task<IActionResult> Delete(Guid? aId)
        {
            if (aId == null)
            {
                return RedirectToAction("Index");
                //                return NotFound();
            }

            var tmpFolder = await _context.Folder
                .FirstOrDefaultAsync(m => m.FolderId == aId);
            if (tmpFolder == null)
            {
                return NotFound();
            }

            return View(tmpFolder);
        }

        // POST: FileStorageArea/Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid aId)
        {
            var tmpFolder = await _context.Folder.FindAsync(aId);
            if (tmpFolder != null)
            {
                await DeleteAllFilesAsync(tmpFolder);
                await DeleteFilesAsync(tmpFolder);

                await DeleteAllFoldersAsync(tmpFolder);
                await DeleteFolderAsync(tmpFolder);

                _context.Folder.Remove(tmpFolder);

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString(SessionCurrentFolderGuid, string.Empty);
                HttpContext.Session.SetString(SessionCurrentElementGuid, string.Empty);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DeleteFolderAsync(Folder aFolder)
        {
            var tmpFolderPath = await GetRealFolderPathAsync(aFolder, aFolder.Name);

            string tmpBaseStr = AppDomain.CurrentDomain.BaseDirectory;

            var tmpFullServerPath = string.Format("{2}{1}{0}", tmpFolderPath, csSystemSeparator, tmpBaseStr);
            var tmpDirectory = new DirectoryInfo(tmpFullServerPath);
            if (tmpDirectory.Exists)
            {
                try
                {
                    System.IO.Directory.Delete(tmpFullServerPath);

                    return true;
                }
                catch (Exception tmpException) when (tmpException is IOException ||
                tmpException is UnauthorizedAccessException ||
                tmpException is ArgumentException ||
                tmpException is ArgumentNullException ||
                tmpException is PathTooLongException ||
                tmpException is DirectoryNotFoundException)
                {
                }
            }

            return false;
        }

        private async Task<bool> DeleteFilesAsync(Folder aFolder)
        {
            var tmpFolderPath = await GetRealFolderPathAsync(aFolder, aFolder.Name);

            string tmpBaseStr = AppDomain.CurrentDomain.BaseDirectory;

            var tmpFullServerPath = string.Format("{2}{1}{0}", tmpFolderPath, csSystemSeparator, tmpBaseStr);
            var tmpDirectory = new DirectoryInfo(tmpFullServerPath);
            if (tmpDirectory.Exists)
            {
                foreach (var tmpFile in System.IO.Directory.GetFiles(tmpDirectory.FullName))
                {
                    try
                    {
                        System.IO.File.Delete(tmpFile);
                        return true;
                    }
                    catch (Exception tmpException) when (tmpException is IOException ||
                    tmpException is UnauthorizedAccessException ||
                    tmpException is ArgumentException ||
                    tmpException is ArgumentNullException ||
                    tmpException is PathTooLongException ||
                    tmpException is NotSupportedException ||
                    tmpException is DirectoryNotFoundException)
                    {
                    }
                }

                return true;
            }

            return false;
        }

        private async Task DeleteAllFoldersAsync(Folder aFolder)
        {
            var tmpChildFolderExist = await _context.Folder.Where(a => a.Parental.FolderId.Equals(aFolder.FolderId)).ToListAsync();
            if (tmpChildFolderExist != null)
            {
                foreach (var tmpFolder in tmpChildFolderExist)
                {
                    await DeleteAllFilesAsync(tmpFolder);
                    await DeleteFilesAsync(tmpFolder);

                    await DeleteAllFoldersAsync(tmpFolder);
                    await DeleteFolderAsync(tmpFolder);

                    _context.Folder.Remove(tmpFolder);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task DeleteAllFilesAsync(Folder aFolder)
        {
            var tmpChildFileExist = await _context.Files.Where(a => a.Folder.FolderId.Equals(aFolder.FolderId)).ToListAsync();
            if (tmpChildFileExist != null)
            {
                foreach (var tmpFile in tmpChildFileExist)
                {
                    _context.Files.Remove(tmpFile);
                }

                await _context.SaveChangesAsync();
            }
        }

        private bool FolderExists(Guid aId)
        {
            return _context.Folder.Any(e => e.FolderId == aId);
        }

        public async Task<IActionResult> SingleFileUpload(Guid? aId)
        {
            if (aId == null)
            {
                return NotFound();
            }

            var tmpFolder = await _context.Folder.FindAsync(aId);
            if (tmpFolder == null)
            {
                return NotFound();
            }
            return View("UploadFile", tmpFolder);
        }

        [HttpPost]
        public async Task<IActionResult> SingleFileUpload(IFormFile SingleFile, Guid aId, string aDescription)
        {
            if (ModelState.IsValid)
            {
                if (SingleFile != null && SingleFile.Length > 0)
                {
                    var tmpFolder = await _context.Folder.Include(a=>a.Parental).FirstOrDefaultAsync(a=>a.FolderId==aId);
                    if (tmpFolder != null)
                    {
                        var tmpFolderPath = await GetRealFolderPathAsync(tmpFolder, tmpFolder.Name);

                        string tmpBaseStr = AppDomain.CurrentDomain.BaseDirectory;

                        var tmpFullServerPath = string.Format("{2}{1}{0}", tmpFolderPath, csSystemSeparator, tmpBaseStr);

                        var tmpDirectory = new DirectoryInfo(tmpFullServerPath);
                        if (tmpDirectory.Exists)
                        {

                            var filePath = Path.Combine(tmpFullServerPath, SingleFile.FileName);

                            //Using Buffering
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                // The file is saved in a buffer before being processed
                                await SingleFile.CopyToAsync(stream);

                                var tmpExt = System.IO.Path.GetExtension(filePath) ?? string.Empty;

                                var tmpFindExtension = await _context.Ext.Where(a => a.Type.Equals(tmpExt)).FirstOrDefaultAsync();
                                if (tmpFindExtension == null)
                                {
                                    //TODO перенести заполнение поддерживаемых расширений файлов в другое место
                                    var tmpExtension = new DB.Model.Extension()
                                    {
                                        ExtensionId = new Guid(),
                                        Type = tmpExt,
                                        Icon = "~/images/banner-right.png"
                                    };

                                    _context.Ext.Add(tmpExtension);
                                    await _context.SaveChangesAsync();

                                    tmpFindExtension = await _context.Ext.Where(a => a.Type.Equals(tmpExt)).FirstAsync();
                                }

                                var tmpFile = new DB.Model.File()
                                {
                                    Folder = tmpFolder,
                                    Name = SingleFile.FileName,
                                    FileId = new Guid(),
                                    Content = SingleFile.ContentType,
                                    Description = aDescription,
                                    Extension = tmpFindExtension,
                                };

                                _context.Files.Add(tmpFile);
                                await _context.SaveChangesAsync();

                            }

                            //Using Streaming
                            //using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            //{
                            //    await SingleFile.CopyToAsync(stream);
                            //}

                            // Process the file here (e.g., save to the database, storage, etc.)
                            return View("UploadSuccess");
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> OnPost(Guid aId)
        {
            int pageCount = 0;

            var tmpFile = await _context.Files.Include(a => a.Extension).Include(a=>a.Folder).FirstOrDefaultAsync(a => a.FileId == aId);
            if (tmpFile?.Folder != null)
            {
                var tmpFolder = await _context.Folder.Include(a => a.Parental).FirstOrDefaultAsync(a => a.FolderId == tmpFile.Folder.FolderId);


                var tmpFolderPath = await GetRealFolderPathAsync(tmpFolder, tmpFolder.Name);

                string tmpBaseStr = AppDomain.CurrentDomain.BaseDirectory;

                var tmpFullServerPath = string.Format("{1}{0}", tmpFolderPath, tmpBaseStr);

                var tmpFullServerFilePath = string.Format("{2}{1}{0}", tmpFile.Name, csSystemSeparator, tmpFullServerPath);

                if (System.IO.File.Exists(tmpFullServerFilePath))
                {
                    var dataFile = System.IO.File.ReadAllLines(tmpFullServerFilePath);
                    if (dataFile != null)
                        return new JsonResult(dataFile);
                }
                //string imageFilesFolder = Path.Combine(outputPath, Path.GetFileName(tmpFile.Name).Replace(".", "_"));
                //if (!Directory.Exists(imageFilesFolder))
                //{
                //    Directory.CreateDirectory(imageFilesFolder);
                //}
                //string imageFilesPath = Path.Combine(imageFilesFolder, "page-{0}.png");
                //using (Viewer viewer = new Viewer(tmpFullServerFilePath))
                //{
                //    //Get document info
                //    ViewInfo info = viewer.GetViewInfo(ViewInfoOptions.ForPngView(false));
                //    pageCount = info.Pages.Count;
                //    //Set options and render document
                //    PngViewOptions options = new PngViewOptions(imageFilesPath);
                //    viewer.View(options);
                //}
            }
            
            return new JsonResult(pageCount);
            }
    }
}
