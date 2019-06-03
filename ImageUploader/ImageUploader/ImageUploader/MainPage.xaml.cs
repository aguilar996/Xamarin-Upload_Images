using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace ImageUploader
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private  async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if(!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error", "This is not supported by your device","OK");
                return;
            }
            var mediaOption = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync();
            if(selectedImage == null)
            {
                await DisplayAlert("Error", "There was an error trying to get your image", "OK");
                return;
            }

            selectedImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
            UploadImage(selectedImageFile.GetStream());
        }

        private async void UploadImage(Stream stream)
        {
            //Creo el Connectionstring
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=imageuploaderjd;AccountKey=qDM51nQcfQpTrYdR1fhwbQOGZ/+aB5eoifXQkOCj5JE22kTvISdl0upxefNl/SRLpfKONTqYLdIzJccxY72zJA==;EndpointSuffix=core.windows.net");
            //nuevo blob client
            var client = account.CreateCloudBlobClient();
            //llamo al container creado en azure
            var container = client.GetContainerReference("imagecontainer");
            await container.CreateIfNotExistsAsync();

            //variable unica de nombre
            var name = Guid.NewGuid().ToString();
            var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
            //con el blob ya creado podemos subir el stream
            await blockBlob.UploadFromStreamAsync(stream); 

            string url = blockBlob.Uri.OriginalString;
        }
    }
}
