﻿using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.Search;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Builders
{
    /// <summary>
    /// Builds a {@link TermQuery} object from a {@link FieldQueryNode} object.
    /// </summary>
    public class FieldQueryNodeBuilder : IStandardQueryBuilder
    {
        public FieldQueryNodeBuilder()
        {
            // empty constructor
        }

        public virtual Query Build(IQueryNode queryNode)
        {
            FieldQueryNode fieldNode = (FieldQueryNode)queryNode;

            return new TermQuery(new Term(fieldNode.GetFieldAsString(), fieldNode
                .GetTextAsString()));
        }
    }
}