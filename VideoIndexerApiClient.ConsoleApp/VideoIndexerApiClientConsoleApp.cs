using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VideoIndexerApiClient.ConsoleApp
{
    class VideoIndexerApiClientConsoleApp
    {


        // TODO: In order to run thise demos, you will need to do the folowing...
        // Create an Azure Storage Account
        // Set the storage account connection variable
        // Create a Video Indexer account
        // Set the location, account id and API key variables


        // Video Indexer Account Details
        static string Location = "Location";
        static string AccountId = "AccountId";
        static string ApiKey = "ApiKey";

        // Storage Account Details
        static string StorageAccountConnectionString = "StorageAccountConnectionString";
        static string VideoContainerName = "video";
        static string AudioContainerName = "audio";

        // ToDo set this variable to a specific media asset id when required.
        static string MediaAssetId = "MediaAssetId";


        static void Main(string[] args)
        {

            Console.WriteLine("Video Indexer API Client Console App");


            //UploadBatchVids(@"C:\Users\sopacifi\Desktop\Videos");

            //SearchAsync("media").Wait();
            //SearchAsync("Microsoft").Wait();
            SearchFaceAsync("Bill Gates").Wait();


            //Authenticating with the Video Indexer API
            //AuthenticateAsync().Wait();

            //ListVideosAsync().Wait();


            //Indexing Content
            //var path = @"FOLDER\FILE";
            //var title = "Summary";
            //var blobName = Path.GetFileName(path);
            //UploadAndIndexMediaAsync(VideoContainerName, path, blobName, title).Wait();




            //Searching Content
            //SearchAsync("microsoft azure").Wait();




            //Playing Content
            //var url = GetVideoPlayerWidgetAsync(MediaAssetId).Result;
            //Console.WriteLine(url);




            //Managing Transcripts using C#
            //DownloadVideoCaptions("900b35a921", @"C:\Users\sopacifi\Desktop\Videos\id.vtt").Wait();
            //UpdateVideoTranscript(MediaAssetId, @"FOLDER_PATH\file.vtt").Wait();




            //Face Detection using C#

            //UploadBatchVids(@"TRAINING VIDEO FOLDER");
            //GetIndex(MediaAssetId).Wait();
            //UpdateVideoFaceAsync(MediaAssetId, FACE_ID, "Name").Wait();


            //GetIndex(MediaAssetId).Wait();

            //UploadBatchVids(@"TEST VIDEO FOLDER");






            Console.WriteLine("Done!");
            Console.ReadLine();
        }


        static async Task UpdateVideoFaceAsync(string videoId, int faceId, string newName)
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, true);

            await client.UpdateVideoFaceAsync(videoId, faceId, newName);
        }


        static async Task GetIndex(string mediaAssetId)
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            await client.GetIndexAsync(mediaAssetId);
        }

        static async Task GetTranslatedIndex()
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            await client.GetIndexAsync(MediaAssetId, "sv-SE");



        }


        static void UploadBatchVids(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                Console.WriteLine($"Uploading: { name }");
                UploadAndIndexMediaAsync(VideoContainerName, file, name, name).Wait();
                Thread.Sleep(10000);
            }
        }


        static async Task ListAccountsAsync()
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, false);
            var accountsJson = await client.GetAccounts();
            Console.WriteLine(accountsJson);
        }




        static async Task AuthenticateAsync()
        {
            // Get an access token with allow edit permissions
            var allowEditClient = new VideoIndexerClient(Location, AccountId, ApiKey, true);
            var allowEditToken = await allowEditClient.GetAccountAccessTokenAsync();
            Console.WriteLine($"Allow edit account token: { allowEditToken }");
            Console.WriteLine();

            // Get a second access token with allow edit permissions
            var allowEditToken2 = await allowEditClient.GetAccountAccessTokenAsync();
            Console.WriteLine($"Allow edit account token: { allowEditToken }");
            Console.WriteLine();

            // Get an access token with read only permissions
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);
            var readOnlyToken = await readOnlyClient.GetAccountAccessTokenAsync();
            Console.WriteLine($"Read only account token: { readOnlyToken }");
            Console.WriteLine();

            // Get a video access token
            var readOnlyVideoAccessToken = await readOnlyClient.GetReadOnlyVideoAccessTokenAsync("0aef5bda16");
            Console.WriteLine($"Read only video access token: { allowEditToken }");
            Console.WriteLine();



        }


        static async Task ListVideosAsync()
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, false);
            var mediaAssets = await client.ListVideosAsync();
            foreach (var mediaAsset in mediaAssets.results)
            {
                Console.WriteLine($"{ mediaAsset.name } - { mediaAsset.durationInSeconds } - { mediaAsset.userName }");
            }
        }



        static async Task UploadAndIndexMediaAsync(string containerName, string path, string blobName, string videoName)
        {
            var container = new MediaAssetsContainer(StorageAccountConnectionString, containerName);

            Console.WriteLine($"Uploading { blobName } to blob storage...");
            await container.UploadFileAsync(path, blobName);

            Console.WriteLine("Creating SAS...");
            string sasUrl = container.GetBlobReadSas(blobName);

            var client = new VideoIndexerClient(Location, AccountId, ApiKey, true);

            Console.WriteLine("Calling API...");
            var response =  await client.IndexAsync(sasUrl, videoName);

            Console.WriteLine();
            Console.WriteLine(response);
            Console.WriteLine();

        }


        static async Task IndexMediaBatch(string containerName, List<Tuple<string, string>> mediaItems)
        {
            var container = new MediaAssetsContainer(StorageAccountConnectionString, containerName);
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, true);

            foreach (var mediaItem in mediaItems)
            {
                Console.WriteLine($"Indexing: { mediaItem }");
                string sasUrl = container.GetBlobReadSas(mediaItem.Item1);
                string result = await client.IndexAsync(sasUrl, mediaItem.Item2);
                Console.WriteLine(result);
                Thread.Sleep(15000);

            }
        }

        static async Task SearchAsync(string searchText)
        {
            // Create a read-only client
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            // Retrieve the search results
            var searcResults = await readOnlyClient.SearchAsync(searchText);

            // Display the search results
            foreach (var result in searcResults.results)
            {
                Console.WriteLine($"{ result.name }");
                foreach (var match in result.searchMatches)
                {
                    Console.WriteLine($"\t{ match.startTime.ToString(@"hh\:mm\:ss") } - { match.type } - { match.text }");
                }
                Console.WriteLine();
            }
        }

        static async Task SearchFaceAsync(string name)
        {
            // Create a read-only client
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            // Retrieve the search results
            var searcResults = await readOnlyClient.SearchFaceAsync(name);

            // Display the search results
            foreach (var result in searcResults.results)
            {
                Console.WriteLine($"{ result.name }");
                foreach (var match in result.searchMatches)
                {
                    Console.WriteLine($"\t{ match.startTime.ToString(@"hh\:mm\:ss") } - { match.type } - { match.text }");
                }
                Console.WriteLine();
            }
        }


        static async Task<string> GetVideoPlayerWidgetAsync(string videoId)
        {
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            string url = await readOnlyClient.GetVideoPlayerWidgetAsync(videoId);

            return url;
        }



        static async Task DownloadVideoCaptions(string videoId, string file)
        {
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            var captions = await readOnlyClient.GetVideoCaptions(videoId, "Vtt");

            File.WriteAllText(file, captions);

            Console.WriteLine(captions);
        }


        static async Task DownloadVideoCaptions(string videoId, string file, string language)
        {
            var readOnlyClient = new VideoIndexerClient(Location, AccountId, ApiKey, false);

            var captions = await readOnlyClient.GetVideoCaptions(videoId, "Vtt", language);

            File.WriteAllText(file, captions);

            Console.WriteLine(captions);
        }


        static async Task UpdateVideoTranscript(string videoId, string file)
        {
            var client = new VideoIndexerClient(Location, AccountId, ApiKey, true);

            string transcript = File.ReadAllText(file);

            await client.UpdateVideoTranscript(videoId, transcript);

            
        }

    }
}
