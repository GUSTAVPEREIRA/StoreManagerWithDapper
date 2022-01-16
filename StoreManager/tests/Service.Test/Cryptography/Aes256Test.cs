using System;
using Bogus;
using Core.Cryptography;
using FluentAssertions;
using Xunit;

namespace Service.Test.Cryptography;

public class Aes256Test
{
    [Fact]
    public void ShouldEncryptOk()
    {
        string password = new Faker().Random.Hash(new Random().Next(10, 15));
        string secret = new Faker().Random.Hash(new Random().Next(5, 10));

        var encrypted = Aes256.EncryptString(password, secret);

        password.Should().NotBe(encrypted);
    }
    
    [Fact]
    public void ShouldDecryptOk()
    {
        string password = new Faker().Random.Hash(new Random().Next(10, 15));
        string secret = new Faker().Random.Hash(new Random().Next(5, 10));

        var encrypted = Aes256.EncryptString(password, secret);
        var decrypted = Aes256.DecryptString(encrypted, secret);

        password.Should().Be(decrypted);
    }
}