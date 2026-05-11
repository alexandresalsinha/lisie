using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Google.GData.Documents;
//using Google.GData.Client;
//using Google.GData.Extensions;
using System.IO;


namespace SpiroStockManagement
{
    static public class SyncingWithGoogle
    {
        

        //private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        //{
        //    // Get the auth URL:
        //    IAuthorizationState state = new AuthorizationState(new[] { TasksService.Scopes.Tasks.GetStringValue() });
        //    state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
        //    Uri authUri = arg.RequestUserAuthorization(state);

        //    // Request authorization from the user (by opening a browser window):
        //    Process.Start(authUri.ToString());
        //    Console.Write("  Authorization Code: ");
        //    string authCode = Console.ReadLine();
        //    Console.WriteLine();

        //    // Retrieve the access token by using the authorization code:
        //    return arg.ProcessUserAuthorization(authCode, state);
        //}
        //#region Google Old Way of Syncing
        //static public void UploadFiles()
        //{
        //    Encoding encoding = Encoding.GetEncoding("ISO-8859-15");

        //    //read and encrypt
        //    string _encrypted2 = GlobalProcedures.base64Encode((Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath)));
        //    FileInfo fileInfo2 = new FileInfo(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "encrypted2");
        //    FileStream stream2 = fileInfo2.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    StreamWriter m_streamWriter2 = new StreamWriter(stream2, encoding);
        //    try
        //    {
        //        m_streamWriter2.Write(_encrypted2);
        //    }
        //    finally
        //    {
        //        m_streamWriter2.Close();
        //        stream2.Close();
        //    }


        //    //read and decrypt
        //    string _decrypted = GlobalProcedures.base64Decode(Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "encrypted2", encoding));
        //    GlobalVariables.SpiroStockManagmentDatabaseProcedures.LoadXmlDocumentFromString(_decrypted);

        //    FileInfo fileInfo22 = new FileInfo(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "decrypted2");
        //    FileStream stream22 = fileInfo22.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    StreamWriter m_streamWriter22 = new StreamWriter(stream22, encoding);
        //    try
        //    {
        //        m_streamWriter22.Write(_decrypted);
        //    }
        //    finally
        //    {
        //        m_streamWriter22.Close();
        //        stream22.Close();
        //    }
        //    return;

        //    string _encrypted = GlobalProcedures.DecodeFrom64(Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "encrypted2", System.Text.UTF32Encoding.UTF32));
        //    //string _encrypted = GlobalProcedures.DecodeFrom64(Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "encrypted2"));
        //    GlobalVariables.SpiroStockManagmentDatabaseProcedures.LoadXmlDocumentFromString(_encrypted);

        //    //string _encrypted = GlobalProcedures.EncodeTo64((Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath, System.Text.UTF32Encoding.UTF32)));

        //    FileInfo fileInfo = new FileInfo(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "encrypted2");
        //    FileStream stream = fileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    StreamWriter m_streamWriter = new StreamWriter(stream);
        //    try
        //    {
        //        m_streamWriter.Write(_encrypted);
        //    }
        //    finally
        //    {
        //        m_streamWriter.Close();
        //        stream.Close();
        //    }
        //    return;
        //    //var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
        //    //FullClientCredentials credentials = PromptingClientCredentials.EnsureFullClientCredentials();
        //    //provider.ClientIdentifier = "656504411799.apps.googleusercontent.com";
        //    //provider.ClientSecret = "339mQ_OJz_HVP9U8Qh_zSFHJ";
        //    //var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

        //    GOAuthRequestFactory requestFactory = new GOAuthRequestFactory("writely", "Docs");

        //    requestFactory.ConsumerKey = "656504411799.apps.googleusercontent.com";
        //    requestFactory.ConsumerSecret = "339mQ_OJz_HVP9U8Qh_zSFHJ";


        //    // Create the DocsService and set its RequestFactory
        //    DocumentsService myService = new DocumentsService(requestFactory.ApplicationName);


        //    myService.RequestFactory = requestFactory;


        //    //DocumentsService myService = new DocumentsService("");
        //    //        myService.setUserCredentials("alexandresalsinha@gmail.com", "@!asd123!@!");
        //    //        GDataGAuthRequestFactory reqFactory = (GDataGAuthRequestFactory)myService.RequestFactory;  
        //    //reqFactory.ProtocolMajor = 3;  
        //    //        DocumentEntry newEntry = myService.UploadDocument(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath, "InventoryItems.xml", "XML");

        //    DocumentEntry entry = null;
        //    fileInfo = new FileInfo(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath);
        //    stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    try
        //    {
        //        Uri postUri = new Uri("https://docs.google.com/feeds/default/private/full?convert=false");
        //        entry = myService.Insert(postUri, stream, "", fileInfo.Name) as DocumentEntry;
        //    }
        //    finally
        //    {
        //        stream.Close();
        //    }
        //}

        //static public void DownloadFiles()
        //{
        //    DocumentsService myService = new DocumentsService("");
        //    myService.setUserCredentials("alexandresalsinha@gmail.com", "@!asd123!@!");
        //    //DocumentsFeed _filesInFolder = RetrieveDocsInFolder(myService, "SpiroDatabases", "alexandresalsinha@gmail.com");

        //    DocumentsListQuery query = new DocumentsListQuery();
        //    query.Title = "InventoryItems.xml";
        //    query.TitleExact = true;
        //    DocumentsFeed feed = myService.Query(query);

        //    Uri documentUri = new Uri(feed.Entries[0].Content.AbsoluteUri);
        //    Stream stream = myService.Query(documentUri);
        //    StreamReader streamReader = new StreamReader(stream);
        //    StreamWriter streamWriter = new StreamWriter(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "googleD");
        //    string line = "";
        //    while ((line = streamReader.ReadLine()) != null)
        //    {
        //        streamWriter.WriteLine(line);
        //    }
        //    streamReader.Close();
        //    streamWriter.Close();

        //}

        //static public void DownloadFile(string nameOfFile, string pathToSave, DocumentsService service)
        //{
        //    DocumentsListQuery query = new DocumentsListQuery();
        //    query.Title = nameOfFile;
        //    query.TitleExact = true;
        //    DocumentsFeed feed = service.Query(query);

        //    Uri documentUri = new Uri(feed.Entries[0].Content.AbsoluteUri);
        //    Stream stream = service.Query(documentUri);
        //    StreamReader streamReader = new StreamReader(stream);
        //    StreamWriter streamWriter = new StreamWriter(GlobalVariables.SpiroStockManagmentDatabaseProcedures.XmlDatabaseFilePath + "googleD");
        //    string line = "";
        //    while ((line = streamReader.ReadLine()) != null)
        //    {
        //        streamWriter.WriteLine(line);
        //    }
        //    streamReader.Close();
        //    streamWriter.Close();
        //}

        //static public DocumentsFeed RetrieveDocsInFolder(DocumentsService service, string folder, string email)
        //{
        //    AtomCategory folderCategory = new AtomCategory(folder,
        //      new AtomUri("http://schemas.google.com/docs/2007/folders/" + email));
        //    QueryCategory folderQueryCategory = new QueryCategory(folderCategory);
        //    DocumentsListQuery query = new DocumentsListQuery();
        //    query.Categories.Add(folderQueryCategory);

        //    return service.Query(query);
        //} 
        //#endregion

        
    }
}
