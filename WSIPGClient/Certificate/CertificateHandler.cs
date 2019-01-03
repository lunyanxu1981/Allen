using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace WSIPGClient.Certificate
{
    /// <summary>
    /// Static class which finds personal certificate of currentUser by subjectName
    /// </summary>
    static class CertificateHandler
    {

        public static X509Certificate2 LoadCertificate(String filePath, String password)
        {
            X509Certificate2 certificate = null;
            try
            {
                certificate = new X509Certificate2(filePath, password);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem loading certificate: " + filePath);
                Console.WriteLine("Exception: " + e.Message);
            }
            return certificate;
        }


        /// <summary>
        /// Finds certificate by SubjectName in personal certificates of currentUser
        /// </summary>
        /// <param name="SubjectName">subjectName of the wanted certificate</param>
        /// <returns>found certificate. Returns null if didn't find any</returns>
        static public X509Certificate2 FindCertificate(String SubjectName)
        {
            if (SubjectName == null || SubjectName == "")
                return null;

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            X509Certificate2 cert = null;
            //Finding certificate by SubjectName
            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindBySubjectName, SubjectName, false);

                if (foundCertificates.Count == 0)
                {
                    // Certificate not found
                    Console.WriteLine("Certificate not found");
                }
                else
                {
                    cert = foundCertificates[0];
                }
            }
            finally
            {
                store.Close();
            }

            return cert;
        }
    }
}
