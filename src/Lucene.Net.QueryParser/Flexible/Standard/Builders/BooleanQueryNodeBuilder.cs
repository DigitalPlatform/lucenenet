﻿using Lucene.Net.QueryParsers.Flexible.Core;
using Lucene.Net.QueryParsers.Flexible.Core.Builders;
using Lucene.Net.QueryParsers.Flexible.Core.Messages;
using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.QueryParsers.Flexible.Messages;
using Lucene.Net.QueryParsers.Flexible.Standard.Parser;
using Lucene.Net.Search;
using System.Collections.Generic;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Builders
{
    /// <summary>
    /// Builds a {@link BooleanQuery} object from a {@link BooleanQueryNode} object.
    /// Every children in the {@link BooleanQueryNode} object must be already tagged
    /// using {@link QueryTreeBuilder#QUERY_TREE_BUILDER_TAGID} with a {@link Query}
    /// object.
    /// <para>
    /// It takes in consideration if the children is a {@link ModifierQueryNode} to
    /// define the {@link BooleanClause}.
    /// </para>
    /// </summary>
    public class BooleanQueryNodeBuilder : IStandardQueryBuilder
    {
        public BooleanQueryNodeBuilder()
        {
            // empty constructor
        }

        public virtual Query Build(IQueryNode queryNode)
        {
            BooleanQueryNode booleanNode = (BooleanQueryNode)queryNode;

            BooleanQuery bQuery = new BooleanQuery();
            IList<IQueryNode> children = booleanNode.GetChildren();

            if (children != null)
            {
                foreach (IQueryNode child in children)
                {
                    object obj = child.GetTag(QueryTreeBuilder<Query>.QUERY_TREE_BUILDER_TAGID);

                    if (obj != null)
                    {
                        Query query = (Query)obj;

                        try
                        {
                            bQuery.Add(query, GetModifierValue(child));
                        }
                        catch (BooleanQuery.TooManyClauses ex)
                        {
                            throw new QueryNodeException(new MessageImpl(
                                QueryParserMessages.TOO_MANY_BOOLEAN_CLAUSES, BooleanQuery
                                    .MaxClauseCount, queryNode
                                    .ToQueryString(new EscapeQuerySyntaxImpl())), ex);
                        }
                    }
                }
            }

            return bQuery;
        }

        private static BooleanClause.Occur GetModifierValue(IQueryNode node)
        {
            if (node is ModifierQueryNode)
            {
                ModifierQueryNode mNode = ((ModifierQueryNode)node);
                switch (mNode.Modifier)
                {
                    case Modifier.MOD_REQ:
                        return BooleanClause.Occur.MUST;

                    case Modifier.MOD_NOT:
                        return BooleanClause.Occur.MUST_NOT;

                    case Modifier.MOD_NONE:
                        return BooleanClause.Occur.SHOULD;
                }
            }

            return BooleanClause.Occur.SHOULD;
        }
    }
}