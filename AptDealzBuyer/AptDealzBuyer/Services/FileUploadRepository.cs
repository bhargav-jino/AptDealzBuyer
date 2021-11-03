using Acr.UserDialogs;
using AptDealzBuyer.API;
using AptDealzBuyer.Model.Request;
using AptDealzBuyer.Repository;
using AptDealzBuyer.Utility;
using System;
using System.Threading.Tasks;

namespace AptDealzBuyer.Services
{
    public class FileUploadRepository : IFileUploadRepository
    {
        public async Task<string> UploadFile(int fileUploadCategory)
        {
            string relativePath = string.Empty;
            string base64String;
            string fileName = string.Empty;

            try
            {
                if (ImageConvertion.SelectedImageByte != null || FileSelection.fileByte != null)
                {
                    if (fileUploadCategory != (int)FileUploadCategory.ProfileDocuments)
                    {
                        base64String = Convert.ToBase64String(ImageConvertion.SelectedImageByte);
                        fileName = Guid.NewGuid().ToString() + ".png";
                    }
                    else if (ImageConvertion.SelectedImageByte != null)
                    {
                        base64String = Convert.ToBase64String(ImageConvertion.SelectedImageByte);
                        fileName = Guid.NewGuid().ToString() + ".png";
                    }
                    else
                    {
                        base64String = Convert.ToBase64String(FileSelection.fileByte);
                        fileName = FileSelection.fileName;
                    }

                    ProfileAPI profileAPI = new ProfileAPI();
                    FileUpload mFileUpload = new FileUpload();

                    mFileUpload.Base64String = base64String;
                    mFileUpload.FileName = fileName;
                    mFileUpload.FileUploadCategory = fileUploadCategory;

                    var mResponse = await profileAPI.FileUpload(mFileUpload);
                    if (mResponse != null && mResponse.Succeeded)
                    {
                        var jObject = (Newtonsoft.Json.Linq.JObject)mResponse.Data;
                        if (jObject != null)
                        {
                            var mBuyerFile = jObject.ToObject<Model.Reponse.BuyerFileDocument>();
                            if (mBuyerFile != null)
                            {
                                relativePath = mBuyerFile.FileUri;
                            }
                        }
                    }
                    else
                    {
                        if (mResponse != null)
                            Common.DisplayErrorMessage(mResponse.Message);
                        else
                            Common.DisplayErrorMessage(Constraints.Something_Wrong);
                    }
                }
                else
                {
                    return relativePath;
                }
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FileUplodeRepository/UplodeFile: " + ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            return relativePath;
        }
    }
}
