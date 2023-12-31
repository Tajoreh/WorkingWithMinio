using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using PasswordGenerator;

namespace Sample.WebApi.Server.Utilities;

public static class Utilities
{
    public static byte[] CreateCertificate(string subjectName, string password)
    {
        //var subjectName = "CN=NotificationCertificate";//My certificate subject
        //var password = SharedData.SharedData.Password;//My password

        //Creates an instance of the default implementation of the RSA algorithm. Create(Int32) Creates a new ephemeral RSA key with the specified key size.
        using var rsa = RSA.Create(2048);

        //The CertificateRequest class allows callers to create self-signed or chain-signed X.509 Public-Key Certificates, as well as to create a certificate signing request blob to send to a Certificate Authority (CA).
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        //This class provides properties that define the basic constraints set on a certificate.
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(certificateAuthority: false, hasPathLengthConstraint: false, pathLengthConstraint: 0, critical: false));

        var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));

        var certData = certificate.Export(X509ContentType.Pkcs12, password);

        

        return certData;
    }

    public static string GeneratePassword()
    {
        var pwd = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: false, includeSpecial: false, passwordLength: 8);
        var password = pwd.Next();

        return password;
    }
}

