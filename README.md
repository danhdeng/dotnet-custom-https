# dotnet-custom-https
# https with dotnet core API

# Certificate Contains

1. the domain name
2. issuing Authority
3. what the Certificate "Authoriises"
4. Issue and Expiration Date
5. Public Key (private key is Secretj)

# HTTPS Request Interaction

client (TLS Version, Cipher Suite & Cliient Random)----> Server
Server(Cipher Suites, Certificate & Server Random) ----> client
Premaster Secret: (Encryted with Public Key)
Session keys Created(with Client Random, Server Random + Premaster) ----> decrypted the premaster with private key
secure information flow flow

# access the API Swagger 

http://localhost:5000/swagger/index.html

# to access the api service

http://localhost:5000/weatherforecast

# setup user ceritificate

1. launch manage user ceritificate from control panel
2. Select Trusted Certification Authorities
3. click on certificates to view all certificates in the local machine


# set up the https localhost development trust
dotnet dev-certs https --trust

# add domain name to hosts file
open hosts file in folder c:\windows\system32\drivers\etc
map IPV4 address with domain name

192.168.2.14 mydomain.com

Ethernet adapter Ethernet:

   Connection-specific DNS Suffix  . : home
   Link-local IPv6 Address . . . . . : 
   IPv4 Address. . . . . . . . . . . : 192.168.2.14
   Subnet Mask . . . . . . . . . . . : 255.255.255.0
   Default Gateway . . . . . . . . . : 192.168.2.1

# run powershell to generate a certificate for localmachine

$cert =New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dns mydomain.com

# setup the password for the certificate
$password =ConvertTo-SecureString -String "P@55w0rd" -Force -AsPlainText

# get the certificate path

$certpath="Cert:\localmachine\my\$($cert.Thumbprint)"

# export the certificate to the local path

Export-PfxCertificate -Cert $certpath -FilePath e:\weather.pfx -Password $password

# add UserSecretId to the project file

    <UserSecretsId>7808304e-aaac-4b64-94c1-1e89cf4725f5</UserSecretsId>

# use dotnet user-secrets to save the certpassword in the secret store C:\Users\Dan\AppData\Roaming\Microsoft\UserSecrets


dotnet user-secrets set "CertPassword" "P@55w0rd"

# add certpath to appsettings.Development.json file

"CertPath": "E:\\weather.pfx"

# define a class to store certificate related information

public class HostConfig{
        public static string CertPath { get; set; }
        
        public static string CertPassword { get; set; }
        
    }

# load certpath and certpassword into HostConfig class object

    .ConfigureServices((context,services)=>{
                    HostConfig.CertPath=context.Configuration["CertPath"];
                    HostConfig.CertPassword=context.Configuration["CertPassword"];
                })

# override the endpoints in kestrel

 webBuilder.UseKestrel(opt=>{
                        opt.ListenAnyIP(5000);
                        opt.ListenAnyIP(5001, listOptions=>{
                            listOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });





