using Minio;
using Minio.DataModel.Args;
using Sample.WebApi.Server.Utilities;

var builder = WebApplication.CreateBuilder(args);



//builder.Services.AddMinio(accessKey, secretKey);
//// 
//// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
//builder.Services.AddMinio(configureClient => configureClient
//.WithEndpoint(endpoint)
//.WithCredentials(accessKey, secretKey)
//.WithSSL(secure)
//    );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/generate", async () =>
{
    try
    {

        var endpoint = "127.0.0.1:9000";
        var accessKey = "Rmx1Az8VlL2jPVmTTSXu"; //"4sBikuqLwFiX3zc2rRxp";
        var secretKey = "9eBXFHd6pDI10CV9AxyPvsoocGKPmcH1LHa9Lqf1"; //"DNyoXDFQWQXJGGCYY4gbH6FiZxqJN8DTrN5yCBvX";
        var secure = false;


        var minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(secure)
            .Build();

        var certificate = Utilities.CreateCertificate("CN=NotificationCertificate", Utilities.GeneratePassword());

        //create object args
        var bucketName = "notification";
        var objectName = Guid.NewGuid().ToString().Replace("-","")+ ".pfx";
        var contentType = "application/octet-stream";

        using var fileStream = new MemoryStream(certificate);
        var poa = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(poa);

        //confirm upload
        var statObjectArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        var objectStat = await minioClient.StatObjectAsync(statObjectArgs);
        Console.WriteLine(objectStat.ObjectName + ": " + objectStat.MetaData + " " + objectStat.VersionId+" "+objectStat.MetaData);


        // Retrieve the object as a stream

        var stream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCallbackStream(x=>x.CopyTo(stream));
        
        await minioClient.GetObjectAsync(getObjectArgs);

        // Convert the stream to byte[]
        byte[] certData = stream.ToArray();

        File.WriteAllBytes("certificate.pfx", certData);


      


    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }

});

app.Run();

