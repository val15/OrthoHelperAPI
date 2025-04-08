# Récupérer la liste des images Ollama installées
$OllamaImages = ollama list | Select-Object -Skip 1 | ForEach-Object {
    $_.Trim().Split(' ')[0]
}

# Vérifier si des images sont installées
if ($OllamaImages.Count -gt 0) {
    Write-Host "Début de la mise à jour de toutes les images Ollama installées..."
    Write-Host "---"

    # Itérer sur chaque image et lancer la mise à jour
    foreach ($Image in $OllamaImages) {
        Write-Host "Mise à jour de l'image : $($Image)"
        ollama pull "$Image"
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Mise à jour de $($Image) terminée."
        } else {
            Write-Host "Erreur lors de la mise à jour de $($Image)."
        }
        Write-Host "---"
    }

    Write-Host "Mise à jour de toutes les images Ollama terminée (ou tentatives effectuées)."
} else {
    Write-Host "Aucune image Ollama installée n'a été trouvée."
}