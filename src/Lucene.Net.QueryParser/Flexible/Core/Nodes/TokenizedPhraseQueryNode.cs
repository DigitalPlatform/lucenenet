﻿using Lucene.Net.QueryParsers.Flexible.Core.Parser;
using System.Collections.Generic;
using System.Text;

namespace Lucene.Net.QueryParsers.Flexible.Core.Nodes
{
    /// <summary>
    /// A {@link TokenizedPhraseQueryNode} represents a node created by a code that
    /// tokenizes/lemmatizes/analyzes.
    /// </summary>
    public class TokenizedPhraseQueryNode : QueryNodeImpl, IFieldableNode
    {
        public TokenizedPhraseQueryNode()
        {
            IsLeaf = false;
            Allocate();
        }

        public override string ToString()
        {
            var children = GetChildren();
            if (children == null || children.Count == 0)
                return "<tokenizedphrase/>";
            StringBuilder sb = new StringBuilder();
            sb.Append("<tokenizedtphrase>");
            foreach (IQueryNode child in children)
            {
                sb.Append("\n");
                sb.Append(child.ToString());
            }
            sb.Append("\n</tokenizedphrase>");
            return sb.ToString();
        }

        // This text representation is not re-parseable
        public override string ToQueryString(IEscapeQuerySyntax escapeSyntaxParser)
        {
            var children = GetChildren();
            if (children == null || children.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            string filler = "";
            foreach (IQueryNode child in children)
            {
                sb.Append(filler).Append(child.ToQueryString(escapeSyntaxParser));
                filler = ",";
            }

            return "[TP[" + sb.ToString() + "]]";
        }

        public override IQueryNode CloneTree()
        {
            TokenizedPhraseQueryNode clone = (TokenizedPhraseQueryNode)base
                .CloneTree();

            // nothing to do

            return clone;
        }

        public virtual string Field
        {
            get
            {
                IList<IQueryNode> children = GetChildren();

                if (children == null || children.Count == 0)
                {
                    return null;

                }
                else
                {
                    return ((IFieldableNode)children[0]).Field;
                }
            }
            set
            {
                IList<IQueryNode> children = GetChildren();

                if (children != null)
                {
                    foreach (IQueryNode child in children)
                    {

                        if (child is IFieldableNode)
                        {
                            ((IFieldableNode)child).Field = value;
                        }
                    }
                }
            }
        }
    }
}