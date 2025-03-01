﻿using System.Collections.Generic;

namespace EncompassRest.Settings.Personas
{
    /// <summary>
    /// PipelineRights
    /// </summary>
    public sealed class PipelineRights : PersonaAccess
    {
        private DirtyList<string> _pipelineTasks;
        private DirtyList<EntityReference> _pipelineViews;
        private DirtyList<NonAccessibleColumn> _nonAccessibleColumns;

        /// <summary>
        /// PipelineRights PipelineTasks
        /// </summary>
        public IList<string> PipelineTasks { get => GetField(ref _pipelineTasks); set => SetField(ref _pipelineTasks, value); }

        /// <summary>
        /// PipelineRights PipelineViews
        /// </summary>
        public IList<EntityReference> PipelineViews { get => GetField(ref _pipelineViews); set => SetField(ref _pipelineViews, value); }

        /// <summary>
        /// PipelineRights NonAccessibleColumns
        /// </summary>
        public IList<NonAccessibleColumn> NonAccessibleColumns { get => GetField(ref _nonAccessibleColumns); set => SetField(ref _nonAccessibleColumns, value); }
    }
}