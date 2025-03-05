using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System.Text;

namespace Tools
{
    public static class TextDiffHelper
    {

        public static string GenerateCharacterDiff(string oldText, string newText)
        {
            var differ = new DiffPlex.Differ();
            var diffs = differ.CreateCharacterDiffs(oldText, newText, ignoreWhitespace: false);
            var sb = new StringBuilder();

            int oldPos = 0;
            int newPos = 0;

            foreach (var diffBlock in diffs.DiffBlocks)
            {
                // Ajouter les caractères identiques avant le bloc de différence
                while (oldPos < diffBlock.DeleteStartA)
                {
                    sb.Append(oldText[oldPos]);
                    oldPos++;
                    newPos++;
                }

                // Traiter les caractères supprimés
                for (int i = 0; i < diffBlock.DeleteCountA; i++)
                {
                    char c = oldText[diffBlock.DeleteStartA + i];
                    sb.Append($"<span style='text-decoration: line-through; color: red;'>{c}</span>");
                }

                // Traiter les caractères ajoutés
                for (int i = 0; i < diffBlock.InsertCountB; i++)
                {
                    char c = newText[diffBlock.InsertStartB + i];
                    sb.Append($"<span style='color: blue;'>{c}</span>");
                }

                oldPos = diffBlock.DeleteStartA + diffBlock.DeleteCountA;
                newPos = diffBlock.InsertStartB + diffBlock.InsertCountB;
            }

            // Ajouter les caractères restants
            while (oldPos < oldText.Length)
            {
                sb.Append(oldText[oldPos]);
                oldPos++;
            }

            // Convertir le résultat en HTML avec mise en forme monospace
            return $@"
<div style='font-family: monospace; white-space: pre-wrap;'>
    {sb.ToString()}
</div>";
        }
    }
}
