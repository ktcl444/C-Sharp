///////////////////////////////////////////////////////////////////////////////
//
// Microsoft Solutions for Security
// Copyright (c) 2004 Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// THE ENTIRE RISK OF USE OR RESULTS IN CONNECTION WITH THE USE OF THIS CODE
// AND INFORMATION REMAINS WITH THE USER.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Security.Cryptography;
using System.Text;

namespace MIISWorkflow
{
	/// <summary>
	/// Utility class for password generation using
	/// cryptographic Random Number Generator
	/// </summary>
	public class PasswordGenerator
	{
		private RNGCryptoServiceProvider    rng;
		private char[] pwdCharArray = 
			( "abcdefghjkmnopqrstuvwxyzABCDEFG" +
				"HJKLMNPQRSTUVWXYZ23456789" ).ToCharArray(); 
		private const int PASSWORD_LENGTH = 8;
		/// <summary>
		/// Implements a cryptographic Random Number Generator 
		/// (RNG)using the implementation provided by the 
		/// cryptographic service provider (CSP).
		/// </summary>
		public PasswordGenerator() 
		{
			rng = new RNGCryptoServiceProvider();			
		}	
	
		/// <summary>
		/// Generates cryptograph random numbers for 
		/// the password.
		/// </summary>
		/// <param name="lBound">Lower Bound value</param>
		/// <param name="uBound">Upper Bound value</param>
		/// <returns>integer</returns>
		protected int GetCryptographicRandomNumber( 
									int lBound, int uBound )
		{   
			// Assumes lBound >= 0 && lBound < uBound
			// returns an int >= lBound and < uBound
			uint urndnum;   
			byte[] rndnum = new Byte[4];   
			if (lBound == uBound-1)  
			{
				// test for degenerate case where only lBound 
				// can be returned
				return lBound;
			}
                                                              
			uint xcludeRndBase = (uint.MaxValue -
				(uint.MaxValue%(uint)(uBound-lBound)));   
        
			do 
			{      
				rng.GetBytes(rndnum);      
                urndnum = System.BitConverter.ToUInt32(rndnum,0);      
			} while (urndnum >= xcludeRndBase);   
        
			return (int)(urndnum % (uBound-lBound)) + lBound;
		}

		/// <summary>
		/// Generate random characters for the password.
		/// </summary>
		/// <returns>Character</returns>
		protected char GetRandomCharacter()
		{            
			int upperBound = pwdCharArray.GetUpperBound(0);

			// call the "GetCryptographicRandomNumber" method
			// for generating random numbers.
			int randomCharPosition = 
				GetCryptographicRandomNumber( 
					pwdCharArray.GetLowerBound(0), upperBound );

			// Get all the characters required for the
			// password generation.
			char randomChar = pwdCharArray[randomCharPosition];

			// return the generated character.
			return randomChar;
		}

		/// <summary>
		/// For any approved contractor, ID will be
		/// created and a corresponding password will
		/// be automatically generated, as a temporary one.
		/// </summary>
		/// <returns>String</returns>
		public string Generate()
		{
			// Pick random length between minimum and maximum   
			// int pwdLength = 8;

			StringBuilder pwdBuffer = new StringBuilder();

			// Gets the maximum number of characters that
			// can be contained in the memory
			pwdBuffer.Capacity = PASSWORD_LENGTH;

			// Generate random characters
			char nextCharacter;

			// Initial dummy character flag
			nextCharacter = '\n';

			for ( int i = 0; i < PASSWORD_LENGTH; i++ )
			{
				// Get the random character
				nextCharacter = GetRandomCharacter();

				// store each character in the pwdBuffer
				pwdBuffer.Append(nextCharacter);				
			}

			//Check for null and return the password string
			if ( pwdBuffer != null )
			{
				return pwdBuffer.ToString();
			}
			else
			{
				return String.Empty;
			}	
		}                                   
	}		
}
