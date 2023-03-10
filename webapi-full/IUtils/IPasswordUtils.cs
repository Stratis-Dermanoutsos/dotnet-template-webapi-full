namespace webapi_full.IUtils;

/// <summary>
/// This interface contains the methods for the encryption utils.
/// It uses the BCrypt API.
/// </summary>
public interface IPasswordUtils
{
    /// <summary>
    /// <paramref name="value" />
    /// <param name="value">: The value to encrypt.</param>
    /// <br />
    /// <returns>Returns the encrypted <paramref name="value" />.</returns>
    /// </summary>
    string Encrypt(string value);

    /// <summary>
    /// <paramref name="value" />
    /// <param name="value">: The value to check.</param>
    /// <br />
    /// <paramref name="encryptedValue" />
    /// <param name="encryptedValue">: The set value.</param>
    /// <br />
    /// <returns>Returns True if the values match.</returns>
    /// </summary>
    bool Check(string value, string encryptedValue);

    /// <summary>
    /// <paramref name="value" />
    /// <param name="value">: The value to validate.</param>
    /// <br />
    /// Throws if the value is invalid based on the rules set.
    /// </summary>
    void Validate(string value);
}