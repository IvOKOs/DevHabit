using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DevHabit.UnitTests.Services;

public class EncryptionServiceTests
{
    private readonly EncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        IOptions<EncryptionOptions> options = Options.Create(new EncryptionOptions
        {
            Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        });

        _encryptionService = new EncryptionService(options);
    }

    [Fact]
    public void Decrypt_ShouldReturnPlainText_WhenDecryptingCorrectCypherText()
    {
        // Arrange
        var plainText = "sensitive information";
        var cipherText = _encryptionService.Encrypt(plainText);

        // Act
        var decryptedCipherText = _encryptionService.Decrypt(cipherText);

        // Assert
        Assert.Equal(plainText, decryptedCipherText);
    }
}
