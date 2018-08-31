using System;

namespace Newtonsoft.Json.Utilities
{
	internal class StringBuffer
	{
		private char[] _buffer;

		private int _position;

		private static readonly char[] _emptyBuffer = new char[0];

		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public StringBuffer()
		{
			_buffer = _emptyBuffer;
		}

		public StringBuffer(int initalSize)
		{
			_buffer = new char[initalSize];
		}

		public void Append(char value)
		{
			if (_position == _buffer.Length)
			{
				EnsureSize(1);
			}
			_buffer[_position++] = value;
		}

		public void Clear()
		{
			_buffer = _emptyBuffer;
			_position = 0;
		}

		private void EnsureSize(int appendLength)
		{
			char[] array = new char[(_position + appendLength) * 2];
			Array.Copy(_buffer, array, _position);
			_buffer = array;
		}

		public override string ToString()
		{
			return ToString(0, _position);
		}

		public string ToString(int start, int length)
		{
			return new string(_buffer, start, length);
		}

		public char[] GetInternalBuffer()
		{
			return _buffer;
		}
	}
}
