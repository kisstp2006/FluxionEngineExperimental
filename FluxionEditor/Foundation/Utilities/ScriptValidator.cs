using System;
using System.Collections.Generic;
using System.IO;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Granular error codes returned by <see cref="ScriptValidator"/>.
    /// </summary>
    public enum ScriptValidationError
    {
        None = 0,

        // ── Name errors ──
        NameEmpty,
        NameTooLong,
        NameInvalidFileNameChars,
        NameStartsWithDigit,
        NameNotCppIdentifier,
        NameCppKeyword,
        NameReservedEngineType,
        NameContainsWhitespace,

        // ── Path errors ──
        PathEmpty,
        PathInvalidChars,
        PathNotAbsolute,
        PathNotExists,

        // ── Conflict ──
        ScriptAlreadyExists,
    }

    /// <summary>
    /// Immutable result from <see cref="ScriptValidator.Validate"/>.
    /// </summary>
    public readonly struct ScriptValidationResult
    {
        public bool IsValid { get; init; }
        public ScriptValidationError Error { get; init; }
        public string Message { get; init; }

        public static ScriptValidationResult Success() => new() { IsValid = true };

        public static ScriptValidationResult Fail(ScriptValidationError error, string message) =>
            new() { IsValid = false, Error = error, Message = message };
    }

    /// <summary>
    /// Stateless validator for script names and paths.
    /// <para>
    /// Usage from a dialog:
    /// <code>
    ///   var r = ScriptValidator.Validate(name, path);
    ///   if (!r.IsValid) { ShowError(r.Message); return; }
    ///   // … create the script …
    /// </code>
    /// For live (keystroke) validation use the overload with
    /// <c>checkExistence: false</c> so you don't hit the disk every keystroke.
    /// </summary>
    public static class ScriptValidator
    {
        // ── Limits ─────────────────────────────────────────────

        /// <summary>Maximum script name length (leaves room for path + extension within MAX_PATH).</summary>
        public const int MaxNameLength = 120;

        // ── C++ keywords (C++23) ───────────────────────────────

        private static readonly HashSet<string> CppKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand", "bitor",
            "bool", "break", "case", "catch", "char", "char8_t", "char16_t", "char32_t",
            "class", "compl", "concept", "const", "consteval", "constexpr", "constinit",
            "const_cast", "continue", "co_await", "co_return", "co_yield", "decltype",
            "default", "delete", "do", "double", "dynamic_cast", "else", "enum",
            "explicit", "export", "extern", "false", "float", "for", "friend", "goto",
            "if", "inline", "int", "long", "mutable", "namespace", "new", "noexcept",
            "not", "not_eq", "nullptr", "operator", "or", "or_eq", "private",
            "protected", "public", "register", "reinterpret_cast", "requires", "return",
            "short", "signed", "sizeof", "static", "static_assert", "static_cast",
            "struct", "switch", "template", "this", "thread_local", "throw", "true",
            "try", "typedef", "typeid", "typename", "union", "unsigned", "using",
            "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq",

            // Common preprocessor / compiler reserved
            "override", "final", "import", "module",
        };

        // ── Engine-reserved type names ─────────────────────────

        private static readonly HashSet<string> EngineReservedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Application", "GameObject", "game_object", "game_object_scripts",
            "component", "Transform", "transform", "Script", "script",
            "Scene", "scene", "Project", "Engine",
            "free_list", "vector", "id_base", "name",
            "v2", "v2a", "v3", "v3a", "v4", "v4a",
            "u32v2", "u32v3", "u32v4", "s32v2", "s32v3", "s32v4",
            "m3x3", "m4x4", "m4x4a",
            "init_info", "game_object_info", "script_id",
            // Common Windows / std names that cause trouble
            "OBJECT", "ERROR", "min", "max", "near", "far",
        };

        // ── Public API ─────────────────────────────────────────

        /// <summary>
        /// Full validation: checks name, path, and optionally file-system conflicts.
        /// </summary>
        /// <param name="name">Script name without extension.</param>
        /// <param name="path">Target folder path.</param>
        /// <param name="checkExistence">
        /// When <c>true</c> verifies the folder exists and no collision on disk.
        /// Set to <c>false</c> for live keystroke validation.
        /// </param>
        public static ScriptValidationResult Validate(string name, string path, bool checkExistence = true)
        {
            // ── Name ──
            var nameResult = ValidateName(name);
            if (!nameResult.IsValid)
                return nameResult;

            // ── Path ──
            var pathResult = ValidatePath(path, checkExistence);
            if (!pathResult.IsValid)
                return pathResult;

            // ── Disk collision ──
            if (checkExistence)
            {
                var collisionResult = CheckFileCollision(name, path);
                if (!collisionResult.IsValid)
                    return collisionResult;
            }

            return ScriptValidationResult.Success();
        }

        /// <summary>Validates only the script name (no disk I/O).</summary>
        public static ScriptValidationResult ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ScriptValidationResult.Fail(ScriptValidationError.NameEmpty,
                    "Script name cannot be empty.");

            var trimmed = name.Trim();

            if (trimmed.Length > MaxNameLength)
                return ScriptValidationResult.Fail(ScriptValidationError.NameTooLong,
                    $"Script name is too long (max {MaxNameLength} characters).");

            if (trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                return ScriptValidationResult.Fail(ScriptValidationError.NameInvalidFileNameChars,
                    "Script name contains invalid characters.");

            if (ContainsWhitespace(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.NameContainsWhitespace,
                    "Script name cannot contain spaces.");

            if (char.IsDigit(trimmed[0]))
                return ScriptValidationResult.Fail(ScriptValidationError.NameStartsWithDigit,
                    "Script name cannot start with a digit.");

            if (!IsValidCppIdentifier(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.NameNotCppIdentifier,
                    "Script name must be a valid C++ identifier (letters, digits, underscores only).");

            if (CppKeywords.Contains(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.NameCppKeyword,
                    $"'{trimmed}' is a C++ reserved keyword and cannot be used as a script name.");

            if (EngineReservedNames.Contains(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.NameReservedEngineType,
                    $"'{trimmed}' is a reserved engine type name and cannot be used as a script name.");

            return ScriptValidationResult.Success();
        }

        /// <summary>Validates only the path (optionally checks disk).</summary>
        public static ScriptValidationResult ValidatePath(string path, bool checkExistence = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                return ScriptValidationResult.Fail(ScriptValidationError.PathEmpty,
                    "Script path cannot be empty.");

            var trimmed = path.Trim();

            if (trimmed.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                return ScriptValidationResult.Fail(ScriptValidationError.PathInvalidChars,
                    "Script path contains invalid characters.");

            if (!Path.IsPathRooted(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.PathNotAbsolute,
                    "Script path must be a fully qualified (absolute) path.");

            if (checkExistence && !Directory.Exists(trimmed))
                return ScriptValidationResult.Fail(ScriptValidationError.PathNotExists,
                    "The selected folder does not exist.");

            return ScriptValidationResult.Success();
        }

        /// <summary>Checks whether .h or .cpp files with the given name already exist.</summary>
        public static ScriptValidationResult CheckFileCollision(string name, string path)
        {
            var headerPath = Path.Combine(path, $"{name}.h");
            var sourcePath = Path.Combine(path, $"{name}.cpp");

            if (File.Exists(headerPath))
                return ScriptValidationResult.Fail(ScriptValidationError.ScriptAlreadyExists,
                    $"A header file named '{name}.h' already exists in this folder.");

            if (File.Exists(sourcePath))
                return ScriptValidationResult.Fail(ScriptValidationError.ScriptAlreadyExists,
                    $"A source file named '{name}.cpp' already exists in this folder.");

            return ScriptValidationResult.Success();
        }

        // ── Helpers ────────────────────────────────────────────

        private static bool IsValidCppIdentifier(string s)
        {
            if (s.Length == 0) return false;
            if (!(char.IsLetter(s[0]) || s[0] == '_')) return false;
            for (int i = 1; i < s.Length; i++)
            {
                if (!(char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                    return false;
            }
            return true;
        }

        private static bool ContainsWhitespace(string s)
        {
            // Faster than LINQ for short strings
            foreach (var c in s)
                if (char.IsWhiteSpace(c))
                    return true;
            return false;
        }
    }
}
