# PFA

## Test FPS

Ouvrir `Assets/Scenes/TestFPS.unity`, lancer Play puis cliquer dans la fenêtre Game.

- ZQSD sur AZERTY : déplacement (WASD sur QWERTY, mêmes positions physiques).
- Souris : regarder autour de soi.
- Espace : sauter lorsqu'on est au sol.
- Shift maintenu : courir.
- Échap : libérer la souris ; clic gauche : reprendre le contrôle.

La scène contient un sol de 50 × 50 m avec collision et damier métrique, un joueur avec CharacterController, sa caméra, une lumière et le volume du template URP. Le joueur revient au point de départ après une chute. Les vitesses, la hauteur de saut et la sensibilité sont réglables sur Player dans l'Inspector.

Unity 6000.3.26f1, Input System 1.20.0, URP 17.3.0. Aucune configuration manuelle requise. La scène initiale est conservée.

Validation : structure de scène et références vérifiées statiquement. Importation, compilation et test Play à effectuer dans Unity ; l'éditeur n'est pas disponible dans l'environnement de préparation.
