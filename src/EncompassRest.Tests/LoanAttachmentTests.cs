﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using EncompassRest.Loans;
using EncompassRest.Loans.Attachments;
using EncompassRest.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncompassRest.Tests
{
    [TestClass]
    public class LoanAttachmentTests : TestBaseClass
    {
        [TestMethod]
        public void LoanAttachment_Serialization()
        {
            var loanAttachment = new LoanAttachment { AttachmentType = AttachmentType.Image };
            Assert.AreEqual(@"{""attachmentType"":1}", loanAttachment.ToJson());
        }

        [TestMethod]
        public async Task LoanAttachments_Upload()
        {
            var client = await GetTestClientAsync();

            var loan = new Loan(client);
            var loanId = await client.Loans.CreateLoanAsync(loan);
            try
            {
                var attachment = new LoanAttachment
                {
                    Title = "Testing Attachment",
                    FileWithExtension = "Text.txt",
                    CreateReason = AttachmentCreateReason.Upload
                };
                var text = "TESTING, TESTING, 1, 2, 3";
                var attachmentId = await loan.Attachments.UploadAttachmentAsync(attachment, Encoding.UTF8.GetBytes(text), true);
                Assert.IsFalse(string.IsNullOrEmpty(attachmentId));
                await Task.Delay(10000);
                var retrievedText = Encoding.UTF8.GetString(await loan.Attachments.DownloadAttachmentAsync(attachmentId));
                Assert.AreEqual(text, retrievedText);
                var stream = await loan.Attachments.DownloadAttachmentStreamAsync(attachmentId);
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    Assert.AreEqual(text, sr.ReadToEnd());
                }

                Assert.IsTrue(string.IsNullOrEmpty(attachment.MediaUrl));
                retrievedText = Encoding.UTF8.GetString(await attachment.DownloadAsync());
                Assert.IsFalse(string.IsNullOrEmpty(attachment.MediaUrl));
                Assert.AreEqual(text, retrievedText);
                stream = await attachment.DownloadStreamAsync();
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    Assert.AreEqual(text, sr.ReadToEnd());
                }

                attachment = await loan.Attachments.GetAttachmentAsync(attachmentId, true);
                Assert.IsFalse(string.IsNullOrEmpty(attachment.MediaUrl));
                retrievedText = Encoding.UTF8.GetString(await attachment.DownloadAsync());
                Assert.AreEqual(text, retrievedText);
                stream = await attachment.DownloadStreamAsync();
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    Assert.AreEqual(text, sr.ReadToEnd());
                }

                var newAttachment = new LoanAttachment
                {
                    Title = "Bob",
                    FileWithExtension = "Bobby.txt",
                    CreateReason = AttachmentCreateReason.Upload
                };
                var newText = "This is a test of the emergency broadcast system, this is only a test.";
                var newAttachmentId = await loan.Attachments.UploadAttachmentAsync(newAttachment, new MemoryStream(Encoding.UTF8.GetBytes(newText)), true);
                Assert.IsFalse(string.IsNullOrEmpty(newAttachmentId));
                await Task.Delay(10000);
                var newRetrievedText = Encoding.UTF8.GetString(await loan.Attachments.DownloadAttachmentAsync(newAttachmentId));
                Assert.AreEqual(newText, newRetrievedText);
                var newStream = await loan.Attachments.DownloadAttachmentStreamAsync(newAttachmentId);
                using (var newSr = new StreamReader(newStream, Encoding.UTF8))
                {
                    Assert.AreEqual(newText, newSr.ReadToEnd());
                }

                Assert.IsTrue(string.IsNullOrEmpty(newAttachment.MediaUrl));
                newStream = await newAttachment.DownloadStreamAsync();
                Assert.IsFalse(string.IsNullOrEmpty(newAttachment.MediaUrl));
                using (var sr = new StreamReader(newStream, Encoding.UTF8))
                {
                    Assert.AreEqual(newText, sr.ReadToEnd());
                }
                newRetrievedText = Encoding.UTF8.GetString(await newAttachment.DownloadAsync());
                Assert.AreEqual(newText, newRetrievedText);

                newAttachment = await loan.Attachments.GetAttachmentAsync(newAttachmentId, true);
                Assert.IsFalse(string.IsNullOrEmpty(newAttachment.MediaUrl));
                newRetrievedText = Encoding.UTF8.GetString(await newAttachment.DownloadAsync());
                Assert.AreEqual(newText, newRetrievedText);
                newStream = await newAttachment.DownloadStreamAsync();
                using (var sr = new StreamReader(newStream, Encoding.UTF8))
                {
                    Assert.AreEqual(newText, sr.ReadToEnd());
                }
            }
            finally
            {
                await Task.Delay(5000);
                await client.Loans.DeleteLoanAsync(loanId);
            }
        }
    }
}
