ConsoleAsync
=========
Restyling of standard C# console with commands creation, multiple consoles and support for asyncronous worker

#Extensions
Extension class with some utility method

##String Methods

	string Left(int length)
>Return the first characters of a string specified by length parameter

	string LeftRest(int length)
Return the last characters of a string without the first characters specified by length parameter, inverse of Left function

`string Right(int length)`
>Return the last characters of a string specified by length parameter


`string RightRest(int length)`
Return the first characters of a string without the last characters specified by length parameter, inverse of Left function

	string Chunk(int startIndex, int endIndex)
>Return subset of string starting and ending from specified parameters

	string Fit(int totalLength, char filler = ' ')
>Return a string with length specified by totalLength parameter, eventually filled or truncated

	string Truncate(int totalLength, string ellipsis = "_")
>Truncate string at the specified length and insert ellipsis character only if the string length is greater than the specified total length

	string RemoveChar(int charIndex)
>Return string without a specified character

	string FitMultiline(int lineLength)
>Cut source string in multiple lines with a length specified by lineLength parameter



##Long Methods

	string ToFileSize()
>Return a string with the long value converted as file size (Eg. 10Bt / 10Mb / 10Gb)
