﻿////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.IO;

namespace RelaxVersioner;

public enum LogImportance
{
    Low = 1,
    Normal = 2,
    High = 3,
    [EditorBrowsable(EditorBrowsableState.Never)]
    Ignore = 100,
}

public abstract class Logger
{
    protected readonly string Header;

    protected Logger(string header) =>
        this.Header = header;

    public abstract void SetImportance(LogImportance lowerImportance);

    public abstract void Message(LogImportance importance, string message);

    public virtual void Message(LogImportance importance, string format, params object?[] args) =>
        this.Message(importance, $"{this.Header}: {string.Format(format, args)}");
    public virtual void Message(LogImportance importance, Exception ex, string format, params object?[] args) =>
        this.Message(importance, $"{this.Header}: {ex.GetType().Name}={ex.Message}, {string.Format(format, args)}");

    public abstract void Warning(string message);

    public virtual void Warning(string format, params object?[] args) =>
        this.Warning($"{this.Header}: {string.Format(format, args)}");
    public virtual void Warning(Exception ex, string format, params object?[] args) =>
        this.Warning($"{this.Header}: {ex.GetType().Name}={ex.Message}, {string.Format(format, args)}");

    public abstract void Error(string message);

    public virtual void Error(string format, params object?[] args) =>
        this.Error($"{this.Header}: {string.Format(format, args)}");
    public virtual void Error(Exception ex, string format, params object?[] args) =>
        this.Error($"{this.Header}: {ex.GetType().Name}={ex.Message}, {string.Format(format, args)}");

    public static Logger Create(string header, LogImportance lowerImportance, TextWriter @out, TextWriter warning, TextWriter error) =>
        new TextWriterLogger(header, lowerImportance, @out, warning, error);
}

internal sealed class TextWriterLogger : Logger
{
    private readonly TextWriter @out;
    private readonly TextWriter warning;
    private readonly TextWriter error;

    private LogImportance lowerImportance;

    public TextWriterLogger(string header, LogImportance lowerImportance, TextWriter @out, TextWriter warning, TextWriter error) :
        base(header)
    {
        this.lowerImportance = lowerImportance;
        this.@out = @out;
        this.warning = warning;
        this.error = error;
    }

    public override void SetImportance(LogImportance lowerImportance) =>
        this.lowerImportance = lowerImportance;

    public override void Message(LogImportance importance, string message)
    {
        if (importance >= this.lowerImportance)
        {
            @out.WriteLine(message);
        }
    }

    public override void Warning(string message) =>
        warning.WriteLine(message);

    public override void Error(string message) =>
        error.WriteLine(message);
}
