using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyConsole.Tools.AzureDevOps.Tests.Data;

public class LinkTypeData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator() {
        return Enum.GetValues(typeof(LinkType)).Cast<LinkType>().Select(s => new object[] { s }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
