﻿using Lucene.Net.QueryParsers.Flexible.Core.Nodes;

namespace Lucene.Net.QueryParsers.Flexible.Core.Parser
{
    /// <summary>
    /// A parser needs to implement {@link SyntaxParser} interface
    /// </summary>
    public interface ISyntaxParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query">query data to be parsed</param>
        /// <param name="field">default field name</param>
        /// <returns>QueryNode tree</returns>
        IQueryNode Parse(string query, string field);
    }
}