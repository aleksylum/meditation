//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Security.Cryptography;
using System.Text;

namespace UI.Logic
{
	public static class HashSha512
	{
		public static Byte[] CalcSha512(String str)
		{
			byte[] res;
			byte[] data = Encoding.UTF8.GetBytes(str);
			using (SHA512 sha = new SHA512Managed())
			{
				res = sha.ComputeHash(data);
			}
			return res;
		}
	}
}
