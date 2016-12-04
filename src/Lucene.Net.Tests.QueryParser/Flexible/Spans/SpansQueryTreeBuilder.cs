﻿using Lucene.Net.QueryParsers.Flexible.Core.Builders;
using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.QueryParsers.Flexible.Standard.Builders;
using Lucene.Net.Search;

namespace Lucene.Net.QueryParsers.Flexible.Spans
{
    /// <summary>
    /// Sets up a query tree builder to build a span query tree from a query node
    /// tree.
    /// <para/>
    /// The defined map is:
    /// - every BooleanQueryNode instance is delegated to the SpanOrQueryNodeBuilder
    /// - every FieldQueryNode instance is delegated to the SpanTermQueryNodeBuilder
    /// </summary>
    public class SpansQueryTreeBuilder : QueryTreeBuilder<Query>, IStandardQueryBuilder
    {
        public SpansQueryTreeBuilder()
        {
            SetBuilder(typeof(BooleanQueryNode), new SpanOrQueryNodeBuilder());
            SetBuilder(typeof(FieldQueryNode), new SpanTermQueryNodeBuilder());

        }

        public override Query Build(IQueryNode queryTree)
        {
            return base.Build(queryTree);
        }
    }
}