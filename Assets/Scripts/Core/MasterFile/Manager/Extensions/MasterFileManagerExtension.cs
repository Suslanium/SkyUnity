namespace Core.MasterFile.Manager.Extensions
{
    public abstract class MasterFileManagerExtension
    {
        protected readonly MasterFileManager MasterFileManager;
        
        protected MasterFileManagerExtension(MasterFileManager masterFileManager)
        {
            MasterFileManager = masterFileManager;
        }
    }
}