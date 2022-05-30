using Arch.CoreLibrary.Entities.CommonModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils
{
    public static class IFormFileUtils
    {
        public static async Task<List<CommonFileModel>> UploadFiles(List<IFormFile> formFiles, int companyId, string foldername)
        {
            var result = new List<CommonFileModel>();
            if (formFiles?.Any() == true)
            {
                foreach (var file in formFiles)
                {
                    if (file == null || file.Length == 0)
                        return null;

                    string fileOriginalName = file.GetFilename();
                    string fileExtension = fileOriginalName.GetExtension();
                    string fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot", "files", foldername,
                                fileName);

                    result.Add(new CommonFileModel
                    {
                        CompanyId = companyId,
                        Extension = fileExtension,
                        FileName = fileName,
                        FileSize = file.Length,
                        FileOriginalName = fileOriginalName,
                        FolderName = foldername,
                        FullPath = path
                    });

                    /// Eğer aynı dosya varsa üzerine yazar
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return result;
        }

        //public static async Task<CommonFileDownloadModel> DownloadFile(string filename, string foldername)
        //{
        //    if (filename == null || foldername == null)
        //        return null;

        //    var path = Path.Combine(
        //                   Directory.GetCurrentDirectory(),
        //                   "wwwroot", "files", foldername, filename);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;

        //    return new CommonFileDownloadModel()
        //    {
        //        Memory = memory,
        //        ContentType = IFormFileUtils.GetContentType(path),
        //        FileName = Path.GetFileName(path)
        //    };

        //    //File(memory, IFormFileUtils.GetContentType(path), Path.GetFileName(path));
        //}

        public static bool DeleteFile(string path)
        {
            try
            {
                // deleted klasörüne taşır.
                var fileName = path.Split('/').Last();
                var destinationPath = path.Replace(fileName, $"deleted/{fileName}");
                File.Move(path, destinationPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetFilename(this IFormFile file)
        {
            return ContentDispositionHeaderValue.Parse(
                            file.ContentDisposition).FileName.ToString().Trim('"');
        }

        public static async Task<MemoryStream> GetFileStream(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream;
        }

        public static async Task<byte[]> GetFileArray(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream.ToArray();
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public static string GetExtension(this string path)
        {
            return Path.GetExtension(path).ToLowerInvariant();
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
