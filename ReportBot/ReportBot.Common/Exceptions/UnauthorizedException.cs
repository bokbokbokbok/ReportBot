﻿namespace ReportBot.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? message)
        : base(message) { }
}