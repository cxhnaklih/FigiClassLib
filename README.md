# FigiClassLib
Figi Interaction Library
This is a library that allows you to use c# to get Figi identifers given sedols, cusips, isins etc (although in the case of cusip and isin 
you should provide a market code).  It works in two modes, one where it will return the full list of Figi identifiers ie one request 
generates many returns and single response mode where one request selects a single figi for your security (using an algorithm).
