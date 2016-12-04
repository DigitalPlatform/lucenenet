﻿using Lucene.Net.QueryParsers.Flexible.Core.Messages;
using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.QueryParsers.Flexible.Messages;
using Lucene.Net.QueryParsers.Flexible.Standard.Parser;
using System;
using System.Collections.Generic;

namespace Lucene.Net.QueryParsers.Flexible.Core.Builders
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// This class should be used when there is a builder for each type of node.
    /// 
    /// <para>
    /// The type of node may be defined in 2 different ways: - by the field name,
    /// when the node implements the <see cref="IFieldableNode"/> interface - by its class,
    /// it keeps checking the class and all the interfaces and classes this class
    /// implements/extends until it finds a builder for that class/interface
    /// </para>
    /// <para>
    /// This class always check if there is a builder for the field name before it
    /// checks for the node class. So, field name builders have precedence over class
    /// builders.
    /// </para>
    /// <para>
    /// When a builder is found for a node, it's called and the node is passed to the
    /// builder. If the returned built object is not <c>null</c>, it's tagged
    /// on the node using the tag <see cref="QueryTreeBuilder.QUERY_TREE_BUILDER_TAGID"/>.
    /// </para>
    /// <para>
    /// The children are usually built before the parent node. However, if a builder
    /// associated to a node is an instance of <see cref="QueryTreeBuilder"/>, the node is
    /// delegated to this builder and it's responsible to build the node and its
    /// children.
    /// </para>
    /// <seealso cref="IQueryBuilder"/>
    /// </summary>
    public class QueryTreeBuilder<TQuery> : IQueryBuilder<TQuery>
    {
        /**
   * This tag is used to tag the nodes in a query tree with the built objects
   * produced from their own associated builder.
   */
        public static readonly string QUERY_TREE_BUILDER_TAGID = typeof(QueryTreeBuilder<TQuery>).Name;

        private IDictionary<Type, IQueryBuilder<TQuery>> queryNodeBuilders;

        private IDictionary<string, IQueryBuilder<TQuery>> fieldNameBuilders;

        /**
         * {@link QueryTreeBuilder} constructor.
         */
        public QueryTreeBuilder()
        {
            // empty constructor
        }

        /**
         * Associates a field name with a builder.
         * 
         * @param fieldName the field name
         * @param builder the builder to be associated
         */
        public virtual void SetBuilder(string fieldName, IQueryBuilder<TQuery> builder)
        {

            if (this.fieldNameBuilders == null)
            {
                this.fieldNameBuilders = new Dictionary<string, IQueryBuilder<TQuery>>();
            }

            this.fieldNameBuilders[fieldName] = builder;
        }

        /**
         * Associates a class with a builder
         * 
         * @param queryNodeClass the class
         * @param builder the builder to be associated
         */
        public virtual void SetBuilder(Type queryNodeClass,
            IQueryBuilder<TQuery> builder)
        {

            if (this.queryNodeBuilders == null)
            {
                this.queryNodeBuilders = new Dictionary<Type, IQueryBuilder<TQuery>>();
            }

            this.queryNodeBuilders[queryNodeClass] = builder;

        }

        private void Process(IQueryNode node)
        {

            if (node != null)
            {
                IQueryBuilder<TQuery> builder = GetBuilder(node);

                if (!(builder is QueryTreeBuilder<TQuery>))
                {
                    IList<IQueryNode> children = node.GetChildren();

                    if (children != null)
                    {

                        foreach (IQueryNode child in children)
                        {
                            Process(child);
                        }

                    }

                }

                ProcessNode(node, builder);

            }

        }

        private IQueryBuilder<TQuery> GetBuilder(IQueryNode node)
        {
            IQueryBuilder<TQuery> builder = null;

            if (this.fieldNameBuilders != null && node is IFieldableNode)
            {
                string field = ((IFieldableNode)node).Field;
                this.fieldNameBuilders.TryGetValue(field, out builder);
            }

            if (builder == null && this.queryNodeBuilders != null)
            {
                Type clazz = node.GetType();

                do
                {
                    builder = GetQueryBuilder(clazz);

                    if (builder == null)
                    {
                        Type[] classes = clazz.GetInterfaces();

                        foreach (Type actualClass in classes)
                        {
                            builder = GetQueryBuilder(actualClass);

                            if (builder != null)
                            {
                                break;
                            }
                        }
                    }
                } while (builder == null && (clazz = clazz.BaseType) != null);
            }

            return builder;
        }

        private void ProcessNode(IQueryNode node, IQueryBuilder<TQuery> builder)
        {
            if (builder == null)
            {
                throw new QueryNodeException(new MessageImpl(
                    QueryParserMessages.LUCENE_QUERY_CONVERSION_ERROR, node
                        .ToQueryString(new EscapeQuerySyntaxImpl()), node.GetType()
                        .Name));
            }

            object obj = builder.Build(node);

            if (obj != null)
            {
                node.SetTag(QUERY_TREE_BUILDER_TAGID, obj);
            }

        }

        private IQueryBuilder<TQuery> GetQueryBuilder(Type clazz)
        {
            if (typeof(IQueryNode).IsAssignableFrom(clazz))
            {
                IQueryBuilder<TQuery> result;
                this.queryNodeBuilders.TryGetValue(clazz, out result);
                return result;
            }

            return null;
        }

        /**
         * Builds some kind of object from a query tree. Each node in the query tree
         * is built using an specific builder associated to it.
         * 
         * @param queryNode the query tree root node
         * 
         * @return the built object
         * 
         * @throws QueryNodeException if some node builder throws a
         *         {@link QueryNodeException} or if there is a node which had no
         *         builder associated to it
         */
        public virtual TQuery Build(IQueryNode queryNode)
        {
            Process(queryNode);

            return (TQuery)queryNode.GetTag(QUERY_TREE_BUILDER_TAGID);
        }
    }
}