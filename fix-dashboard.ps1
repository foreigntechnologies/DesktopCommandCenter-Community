$xaml = Get-Content -Raw "src\DesktopCommandCenter.UI\Views\DashboardPage.xaml"
$startToken = "<!-- ══ FERRAMENTAS EM DESTAQUE (Grid de Cards) ══ -->"
$endToken = "<!-- ══ RODAPÉ: Links do app ══ -->"

$idxStart = $xaml.IndexOf($startToken)
$idxEnd = $xaml.IndexOf($endToken)

if ($idxStart -ge 0 -and $idxEnd -gt $idxStart) {
    $before = $xaml.Substring(0, $idxStart)
    $after = $xaml.Substring($idxEnd)

    $newXaml = @"
<!-- ══ FERRAMENTAS DA COMUNIDADE (GRATUITO) ══ -->
            <StackPanel Spacing="12">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>
                    <TextBlock Grid.Column="0" Text="Ferramentas da Comunidade" Style="{StaticResource SubtitleTextBlockStyle}" FontWeight="SemiBold" />
                </Grid>

                <!-- Grade 4 colunas (Community) -->
                <Grid ColumnSpacing="12" RowSpacing="12">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>

                    <!-- Row 1 -->
                    <Button Grid.Row="0" Grid.Column="0" Click="ToolCard_Click" Tag="ColorPicker" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A0099DA" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xEF3C;" FontSize="20" Foreground="#0099DA" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Seletor de Cores" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Captura cores da tela" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="0" Grid.Column="1" Click="ToolCard_Click" Tag="Clipboard" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A7B68EE" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xF0E3;" FontSize="20" Foreground="#7B68EE" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Clipboard" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Histórico de cópias" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="0" Grid.Column="2" Click="ToolCard_Click" Tag="Notes" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AFFB347" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE70B;" FontSize="20" Foreground="#FFB347" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Notas" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Anotações rápidas" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="0" Grid.Column="3" Click="ToolCard_Click" Tag="Calculadora" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A4CAF50" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE8EF;" FontSize="20" Foreground="#4CAF50" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Calculadora" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Científica, física, química" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <!-- Row 2 -->
                    <Button Grid.Row="1" Grid.Column="0" Click="ToolCard_Click" Tag="Awake" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AFF9800" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE708;" FontSize="20" Foreground="#FF9800" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Modo Ativo" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Mantém o PC acordado" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="1" Grid.Column="1" Click="ToolCard_Click" Tag="AlwaysOnTop" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A4FC3F7" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE898;" FontSize="20" Foreground="#4FC3F7" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Sempre no Topo" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Fixa janelas na frente" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="1" Grid.Column="2" Click="ToolCard_Click" Tag="Tradutor" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A26C6DA" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xF2B7;" FontSize="20" Foreground="#26C6DA" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Tradutor" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Tradução instantânea" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="1" Grid.Column="3" Click="ToolCard_Click" Tag="Temporizador" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AFF7043" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE916;" FontSize="20" Foreground="#FF7043" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Temporizador" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Cronômetro e timer" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <!-- Row 3 -->
                    <Button Grid.Row="2" Grid.Column="0" Click="ToolCard_Click" Tag="SystemUpdates" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AE53935" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE895;" FontSize="20" Foreground="#E53935" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Update Center" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Atualizações de apps" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="2" Grid.Column="1" Click="ToolCard_Click" Tag="PesquisaUniversal" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A42A5F5" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE721;" FontSize="20" Foreground="#42A5F5" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Pesquisa Universal" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Busca em tudo" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="2" Grid.Column="2" Click="ToolCard_Click" Tag="CliCommands" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AEF5350" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE756;" FontSize="20" Foreground="#EF5350" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="Paleta de Comandos" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Comandos CLI rápidos" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="2" Grid.Column="3" Click="ToolCard_Click" Tag="FutureShell" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A7B68EE" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE756;" FontSize="20" Foreground="#7B68EE" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <TextBlock Text="FutureShell" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                <TextBlock Text="Terminal Independente" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>
                </Grid>
            </StackPanel>

            <!-- ══ RECURSOS PRO ══ -->
            <StackPanel Spacing="12" Margin="0,32,0,0">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>
                    <TextBlock Grid.Column="0" Text="Recursos PRO (IA &amp; Automação)" Style="{StaticResource SubtitleTextBlockStyle}" FontWeight="SemiBold" />
                </Grid>

                <Grid ColumnSpacing="12" RowSpacing="12">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>

                    <Button x:Name="BtnProChatFT" Grid.Row="0" Grid.Column="0" Click="ToolCard_Click" Tag="ChatFT" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AFF9800" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE9F5;" FontSize="20" Foreground="#FF9800" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <StackPanel Orientation="Horizontal" Spacing="6">
                                    <TextBlock x:Name="TxtPro1Title" Text="ChatFT" Style="{StaticResource BodyStrongTextBlockStyle}" />
                                    <Border Background="#FF9800" CornerRadius="4" Padding="5,1" Visibility="{x:Bind ViewModel.InverseProVisibility, Mode=OneWay}">
                                        <TextBlock Text="PRO" FontSize="9" FontWeight="Bold" Foreground="White" />
                                    </Border>
                                </StackPanel>
                                <TextBlock x:Name="TxtPro1Desc" Text="Transcrição e chat offline" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button Grid.Row="0" Grid.Column="1" Click="ToolCard_Click" Tag="Prompts" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AAB47BC" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE8BD;" FontSize="20" Foreground="#AB47BC" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <StackPanel Orientation="Horizontal" Spacing="6">
                                    <TextBlock Text="Prompts de IA" Style="{StaticResource BodyStrongTextBlockStyle}" TextWrapping="NoWrap" />
                                    <Border Background="#FF9800" CornerRadius="4" Padding="5,1" Visibility="{x:Bind ViewModel.InverseProVisibility, Mode=OneWay}">
                                        <TextBlock Text="PRO" FontSize="9" FontWeight="Bold" Foreground="White" />
                                    </Border>
                                </StackPanel>
                                <TextBlock Text="Biblioteca de prompts" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button x:Name="BtnProAutomacoes" Grid.Row="0" Grid.Column="2" Click="ToolCard_Click" Tag="Automacoes" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A00BCD4" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xED35;" FontSize="20" Foreground="#00BCD4" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <StackPanel Orientation="Horizontal" Spacing="6">
                                    <TextBlock x:Name="TxtPro2Title" Text="Automações" Style="{StaticResource BodyStrongTextBlockStyle}" />
                                    <Border Background="#FF9800" CornerRadius="4" Padding="5,1" Visibility="{x:Bind ViewModel.InverseProVisibility, Mode=OneWay}">
                                        <TextBlock Text="PRO" FontSize="9" FontWeight="Bold" Foreground="White" />
                                    </Border>
                                </StackPanel>
                                <TextBlock x:Name="TxtPro2Desc" Text="Workflows automatizados" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button x:Name="BtnProCloudSync" Grid.Row="0" Grid.Column="3" Click="ToolCard_Click" Tag="CloudSync" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1AE91E63" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE753;" FontSize="20" Foreground="#E91E63" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <StackPanel Orientation="Horizontal" Spacing="6">
                                    <TextBlock x:Name="TxtPro3Title" Text="Cloud Sync" Style="{StaticResource BodyStrongTextBlockStyle}" />
                                    <Border Background="#FF9800" CornerRadius="4" Padding="5,1" Visibility="{x:Bind ViewModel.InverseProVisibility, Mode=OneWay}">
                                        <TextBlock Text="PRO" FontSize="9" FontWeight="Bold" Foreground="White" />
                                    </Border>
                                </StackPanel>
                                <TextBlock x:Name="TxtPro3Desc" Text="Sincronize notas e dados" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>

                    <Button x:Name="BtnProPerfis" Grid.Row="1" Grid.Column="0" Click="ToolCard_Click" Tag="Perfis" Style="{StaticResource ToolCardButtonStyle}">
                        <Grid Padding="16,14">
                            <Grid.ColumnDefinitions><ColumnDefinition Width="Auto"/><ColumnDefinition Width="*"/></Grid.ColumnDefinitions>
                            <Border Grid.Column="0" Width="40" Height="40" CornerRadius="10" Background="#1A8BC34A" Margin="0,0,12,0">
                                <FontIcon Glyph="&#xE713;" FontSize="20" Foreground="#8BC34A" />
                            </Border>
                            <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="2">
                                <StackPanel Orientation="Horizontal" Spacing="6">
                                    <TextBlock x:Name="TxtPro4Title" Text="Perfis" Style="{StaticResource BodyStrongTextBlockStyle}" />
                                    <Border Background="#FF9800" CornerRadius="4" Padding="5,1" Visibility="{x:Bind ViewModel.InverseProVisibility, Mode=OneWay}">
                                        <TextBlock Text="PRO" FontSize="9" FontWeight="Bold" Foreground="White" />
                                    </Border>
                                </StackPanel>
                                <TextBlock x:Name="TxtPro4Desc" Text="Múltiplos perfis de uso" FontSize="11" Foreground="{ThemeResource TextFillColorSecondaryBrush}" TextWrapping="NoWrap" />
                            </StackPanel>
                        </Grid>
                    </Button>
                </Grid>
            </StackPanel>
        </StackPanel>
    </ScrollViewer>

    <!-- ══ RODAPÉ: Links do app ══ -->
"@

    Set-Content -Path "src\DesktopCommandCenter.UI\Views\DashboardPage.xaml" -Value ($before + $newXaml + $after)
    Write-Output "Dashboard atualizado com sucesso."
} else {
    Write-Output "Tokens não encontrados."
}
