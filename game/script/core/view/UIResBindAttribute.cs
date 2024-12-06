using System;

namespace core.view;

[AttributeUsage(AttributeTargets.Class)]
public class UIResBindAttribute(string path) : Attribute
{
    public string Path = path;
}