# PFA

## Arène FPS

Ouvrir `Assets/Scenes/TestFPS.unity`, lancer Play, puis cliquer dans Game. Le sol, les plateformes, les rampes, les couvertures et les repères colorés sont présents dans la scène et modifiables dans l'éditeur. Le pistolet, son Canvas de munitions, l'ennemi et le HUD sont construits au lancement à partir des primitives Unity.

| Contrôle | Action |
| --- | --- |
| ZQSD sur AZERTY | Se déplacer (positions physiques WASD) |
| Souris | Regarder |
| Espace | Sauter |
| Shift maintenu | Courir |
| Clic gauche | Tirer, un coup par appui |
| R | Recharger en 1,2 seconde, réserve illimitée |
| Échap | Libérer le curseur ; cliquer pour reprendre |

### Terrain

Sol de 50 × 50 m. Deux plateformes à 3 m, accessibles depuis le sol par des rampes de 4 m de large, rejoignent une passerelle à 6 m par deux autres rampes. Les accès ouest sont bleus, les accès est rouges. Les blocs servent de couverture et les tours de repères. Les chemins au sol et sous la passerelle restent praticables.

### Combat

- Un garde rouge avec casque, visière et matraque poursuit le joueur. Le NavMesh est construit au lancement uniquement à partir des collisions du terrain. Le garde recalcule sa destination toutes les 0,15 s et emprunte un chemin navigable vers le joueur, rampes comprises. Il ne traverse pas les murs et ne saute pas.
- Une frappe à moins de 1,65 m, sans obstacle et à hauteur compatible, retire 20 PV, avec un délai d'une seconde entre deux frappes.
- Pistolet bleu et noir à 6 cartouches. Son compteur `6/6` est un Canvas World Space enfant du pistolet et suit son recul et la caméra. Jaune pendant le rechargement, rouge quand le chargeur est vide.
- Les projectiles jaunes se déplacent à 65 m/s. Un balayage sphérique entre les positions évite de traverser les colliders à grande vitesse. Chaque projectile s'arrête au premier obstacle. Trois impacts tuent le garde ; un nouveau apparaît une seconde plus tard à un point navigable éloigné du joueur.
- Joueur à 100 PV. Les bords rouges deviennent plus opaques à mesure que les PV baissent. À zéro, les déplacements et les tirs sont bloqués, `You are dead` apparaît pendant 3 secondes, puis le joueur revient au départ avec 100 PV et 6 cartouches. L'ennemi et les projectiles sont réinitialisés. Aucun soin automatique.

### Réglages

Les vitesses et la sensibilité sont sur `Player > FirstPersonController`, les PV et le délai de retour sur `Player > PlayerHealth`, le chargeur et le rechargement sur `Player > Pistol`. La vitesse du garde se règle sur `Arena Game > ArenaGame` ; ses dégâts, sa portée et son rythme sont exposés dans `MeleeEnemy` sur le garde en Play.

Unity 6000.3.26f1, Input System 1.20.0, AI Navigation 2.0.15, URP 17.3.0 : les packages nécessaires sont déjà dans le projet.

### Vérification

Structure YAML, références de scène et GUID, connexions géométriques des rampes et logique de dégâts vérifiés statiquement. Unity Editor n'est pas installé dans l'environnement de préparation : la compilation, le bake NavMesh, l'affichage et les combats doivent encore être vérifiés en Play dans Unity.
