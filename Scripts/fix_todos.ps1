function Replace-TODO {
    param($File, $Type, $Property)
    $content = Get-Content $File -Raw
    
    # Regex to match: // TODO: Implement translation for {ElementName} of type {Type}
    $pattern = "(?m)^\s*// TODO: Implement translation for (\w+) of type $Type\s*$"
    
    $content = [regex]::Replace($content, $pattern, {
        param($match)
        $element = $match.Groups[1].Value
        # Deduce the key. E.g. "TemporizadorInputYearsElement" -> remove "Element", then usually it's "Temporizador_InputYears"
        # However, it's safer to just look up the element in the XAML? 
        # Wait, the translation keys are typically already in the json files or we need to add them!
        return "            $element.$Property = Helpers.LocalizationHelper.Instance.GetString(`"MISSING_KEY_$element`");"
    })
    
    Set-Content $File -Value $content -Encoding UTF8
}
