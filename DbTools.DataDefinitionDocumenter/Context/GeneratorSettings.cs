﻿namespace FizzCode.DbTools.DataDefinitionDocumenter;

public class GeneratorSettings : DocumenterSettingsBase
{
    public bool ShouldCommentOutColumnsWithFkReferencedTables { get; set; }
    public bool ShouldCommentOutFkReferences { get; set; }

    public bool ShouldUseStoredProceduresFromQueries { get; set; }
}
