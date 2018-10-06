Imports System
Imports System.IO
Imports System.text
Imports System.Security.Cryptography
Imports System.Web.Caching


Public Class Security

	'*******************************************************************************************
	'* PlainText is the text you are going to encrypt. Password is the 'Salt' - an added bit   *
	'* of information to further confuse a hacker. Normally this would be different and unique *
	'* for each encryption. (CustomerID, OrderDate - something that would never change.)       *
	'*  The key is a unique set of bytes used by the algorithm to generate the encryption. You *
	'*  have to have the same key to decrypt.                                                  *
	'*******************************************************************************************

	Public Function Encrypt(ByVal PlainText As String) As String
		' Encrypt = Encrypt(PlainText, "96606658-e2f6-41c6-b40c-b667f1c6f553")
        Encrypt = Encrypt(PlainText, "b1ca5b6f-0a36-4aab-a0ed-3ec315570317")
	End Function

	Public Function Decrypt(ByVal EncryptedText As String) As String
		'Decrypt = Decrypt(EncryptedText, "96606658-e2f6-41c6-b40c-b667f1c6f553")
        Decrypt = Decrypt(EncryptedText, "b1ca5b6f-0a36-4aab-a0ed-3ec315570317")
	End Function

	Private Function Encrypt(ByVal clearData As Byte(), ByVal Key As Byte(), ByVal IV As Byte()) As Byte()
		Dim ms As MemoryStream = New MemoryStream
		Dim alg As Rijndael = Rijndael.Create()
		alg.Key = Key
		alg.IV = IV
		Dim cs As CryptoStream = New CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write)
		cs.Write(clearData, 0, clearData.Length)
		cs.Close()
		Dim encryptedData As Byte() = ms.ToArray()
		Return encryptedData
	End Function

	Private Function Encrypt(ByVal clearText As String, ByVal Password As String) As String
		Dim clearBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(clearText)
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}	'This is the key
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		' Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Dim EncryptedData As Byte() = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16))
		Return Convert.ToBase64String(EncryptedData)

	End Function

	Private Function Encrypt(ByVal clearData As Byte(), ByVal Password As String) As Byte()
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}	'this is the key
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		' Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16))
	End Function

	Private Sub Encrypt(ByVal fileIn As String, ByVal fileout As String, ByVal Password As String)
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}	'this is the key
		Dim fsIn As FileStream = New FileStream(fileIn, FileMode.Open, FileAccess.Read)
		Dim fsOut As FileStream = New FileStream(fileout, FileMode.OpenOrCreate, FileAccess.Write)
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		' Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Dim alg As Rijndael = Rijndael.Create()
		alg.Key = pdb.GetBytes(32)
		alg.IV = pdb.GetBytes(32)
		Dim cs As CryptoStream = New CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write)
		Dim bufferlen As Integer = 4096
		Dim buffer As Byte() = {}
		Dim bytesRead As Integer
		Do While (bytesRead <> 0)
			bytesRead = fsIn.Read(buffer, 0, bufferlen)
			cs.Write(buffer, 0, bytesRead)
		Loop
		cs.Close()
		fsIn.Close()
	End Sub

	Private Function Decrypt(ByVal cipherData As Byte(), ByVal Key As Byte(), ByVal IV As Byte()) As Byte()
		Dim ms As MemoryStream = New MemoryStream
		Dim alg As Rijndael = Rijndael.Create()
		alg.Key = Key
		alg.IV = IV
		Dim cs As CryptoStream = New CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write)
		cs.Write(cipherData, 0, cipherData.Length)
		cs.Close()
		Dim decryptedData As Byte() = ms.ToArray()
		Return decryptedData
	End Function

	Private Function Decrypt(ByVal cipherText As String, ByVal Password As String) As String
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}	'this is the key
		Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		'Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Dim decryptedData As Byte() = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16))
		Return System.Text.Encoding.Unicode.GetString(decryptedData)
	End Function

	Private Function Decrypt(ByVal cipherData As Byte(), ByVal Password As String) As Byte()
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}	'this is the key
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		'Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16))
	End Function

	Private Sub Decrypt(ByVal fileIn As String, ByVal fileOut As String, ByVal Password As String)
		Dim NewByte As Byte() = {49, 110, 201, 148, 219, 6, 19, 168, 142, 212, 196, 196, 2}
		Dim fsIn As FileStream = New FileStream(fileIn, FileMode.Open, FileAccess.Read)
		Dim fsOut As FileStream = New FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write)
		Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Password, NewByte)
		'Dim pdb As PasswordDeriveBytes = New PasswordDeriveBytes(Password, NewByte)
		Dim alg As Rijndael = Rijndael.Create()
		alg.Key = pdb.GetBytes(32)
		alg.IV = pdb.GetBytes(16)
		Dim cs As CryptoStream = New CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write)
		Dim bufferLen As Integer = 4096
		Dim buffer As Byte() = {}
		Dim bytesRead As Integer

		Do While bytesRead <> 0
			bytesRead = fsIn.Read(buffer, 0, bufferLen)
			cs.Write(buffer, 0, bytesRead)
		Loop
		cs.Close()
		fsIn.Close()
	End Sub

	' Hash an input string and return the hash as
	' a 32 character hexadecimal string.
	Public Function getMd5Hash(ByVal input As String, ByVal salt As String) As String
		' Create a new instance of the MD5CryptoServiceProvider object.
		Dim md5Hasher As New MD5CryptoServiceProvider()

		' Convert the input string to a byte array and compute the hash.
		Dim data As Byte() = md5Hasher.ComputeHash(Encoding.Default.GetBytes(salt & input))

		' Create a new Stringbuilder to collect the bytes
		' and create a string.
		Dim sBuilder As New StringBuilder()

		' Loop through each byte of the hashed data 
		' and format each one as a hexadecimal string.
		Dim i As Integer
		For i = 0 To data.Length - 1
			sBuilder.Append(data(i).ToString("x2"))
		Next i

		' Return the hexadecimal string.
		Return sBuilder.ToString()

	End Function

End Class
