using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretEncryption;

namespace secret_encryption.App.Test
{
    /// <summary>
    /// SecretEncryptionProgramTest testing for SecretEncryptionProgram
    /// author: Gregory Green
    /// </summary>
   [TestClass]
   public class SecretEncryptionProgramTest
   {
       [TestMethod]
       public void Cryption()
       {
           System.Environment.SetEnvironmentVariable("CRYPTION_KEY","UNIT_TEST");
            string[] args = {};
            Assert.IsNotNull(SecretEncryptionProgram.EncryptSecret("hello"));
       }
       
   }
}