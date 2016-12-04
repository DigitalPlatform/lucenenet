﻿using Lucene.Net.QueryParsers.Flexible.Core.Util;
using System.Collections.Generic;

namespace Lucene.Net.QueryParsers.Flexible.Core.Config
{
    /// <summary>
    /// This class can be used to hold any query configuration and no field
    /// configuration. For field configuration, it creates an empty
    /// {@link FieldConfig} object and delegate it to field config listeners, 
    /// these are responsible for setting up all the field configuration.
    /// <para>
    /// {@link QueryConfigHandler} should be extended by classes that intends to
    /// provide configuration to {@link QueryNodeProcessor} objects.
    /// </para>
    /// <para>
    /// The class that extends {@link QueryConfigHandler} should also provide
    /// {@link FieldConfig} objects for each collection field.
    /// </para>
    /// </summary>
    /// <seealso cref="FieldConfig"/>
    /// <seealso cref="IFieldConfigListener"/>
    /// <seealso cref="QueryConfigHandler"/>
    public abstract class QueryConfigHandler : AbstractQueryConfig
    {
        private readonly LinkedList<IFieldConfigListener> listeners = new LinkedList<IFieldConfigListener>();

        /**
         * Returns an implementation of
         * {@link FieldConfig} for a specific field name. If the implemented
         * {@link QueryConfigHandler} does not know a specific field name, it may
         * return <code>null</code>, indicating there is no configuration for that
         * field.
         * 
         * @param fieldName
         *          the field name
         * @return a {@link FieldConfig} object containing the field name
         *         configuration or <code>null</code>, if the implemented
         *         {@link QueryConfigHandler} has no configuration for that field
         */
        public virtual FieldConfig GetFieldConfig(string fieldName)
        {
            FieldConfig fieldConfig = new FieldConfig(StringUtils.ToString(fieldName));

            foreach (IFieldConfigListener listener in this.listeners)
            {
                listener.BuildFieldConfig(fieldConfig);
            }

            return fieldConfig;

        }

        /**
         * Adds a listener. The added listeners are called in the order they are
         * added.
         * 
         * @param listener
         *          the listener to be added
         */
        public virtual void AddFieldConfigListener(IFieldConfigListener listener)
        {
            this.listeners.AddLast(listener);
        }
    }
}