using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

enum ReadMode { TEXT, CONDITION, OPERATION, VALUE, NEXT_ADRESS, NEXT_DESC }
enum Condition { EQUALS, GREATER_THAN }
public enum OperationType { NONE, ASSIGN, INCREMENT}

public struct Operation
{
	public string key;
	public float value;
	public OperationType operationType;
}

public class GameEvent{

	public string fileName;

	static public GameObject textBox;
	static public GameObject dialogueBox;

	int currentBox = 0;
	
	public string text = "Texte non chargé";
	public List<GameEvent> nextGameEvents = new List<GameEvent>();
	public List<string> nextDescriptions = new List<string>();
	public List<HashSet<Operation> > nextOperations = new List<HashSet<Operation> >();
	public List<GameObject> boxes = new List<GameObject>();


	public void Load()
	{

		string loadedText = "";

		string nstring = "";
		string dstring = "";
		string cstring = "";
		string vstring = "";
		string ostring = "";
		string ostring2 = "";

		float value1 = 0;
		float value2 = 0;

		int nbOfReturn = 0;

		int i = 0;

		Condition condition = Condition.EQUALS;
		OperationType operationType = OperationType.NONE;
		int currentConditionLayer = 0;
		int maxVerifiedConditionLayer = 0;
		StreamReader reader = new StreamReader(fileName);

		ReadMode mode = ReadMode.TEXT;
		ReadMode prevMode = ReadMode.TEXT;

		bool readingDesc = false;

		char c;
		while (!reader.EndOfStream)
		{
			//Debug.Log(mode);
			do
			{
				c = (char)reader.Read();
				i++;
				if (c == '{')
				{
					currentConditionLayer++;
				}
				else if (c == '}')
				{
					if (currentConditionLayer == 0)
					{
						Debug.Log( "Erreur : '}' sans condition préalable au caractère" + i);
					}
					if (currentConditionLayer == maxVerifiedConditionLayer)
					{
						maxVerifiedConditionLayer--;
					}
					currentConditionLayer--;

				}
			} while (currentConditionLayer > maxVerifiedConditionLayer);


			Debug.Log(c);
			Debug.Log((int)c);
			switch (c)
			{
				case '£'://Lecture de condition
					if (mode == ReadMode.CONDITION)//Fermeture de la lecture de condition
					{
						bool conditionUnknown = false;
						if (!float.TryParse(cstring, out value2))
						{
							if (!GameManager.values.TryGetValue(cstring, out value2))
							{
								Debug.Log( "Clé inconnue " + cstring + " au caractère " + i);
								//Si valeur inconnue, on considère que la condition n'est pas vérifiée
								conditionUnknown = true;
							}
						}
						if (!conditionUnknown && IsVerified(value1, value2, condition)) //Si la condition est connue
						{
							maxVerifiedConditionLayer++;
						}
						mode = prevMode;
					}
					else //Ouverture de la lecture de condition
					{
						prevMode = mode;
						cstring = "";
						mode = ReadMode.CONDITION;
					}
					break;
				case '$': //Lecture de valeur
					if (mode == ReadMode.VALUE)//Fermeture de la lecture de valeur
					{
						string s;
						if (!GameManager.names.TryGetValue(vstring, out s)) //On vérifie si un nom est associé à la clé
						{
							float v;
							if (!GameManager.values.TryGetValue(vstring, out v)) //Sinon, si une valeur y est associée
							{
								Debug.Log("Erreur : clé inconnue " + vstring + " au caractère " + i);
							}
							s = v.ToString();
						} 
						//On ajoute la valeur au texte
						loadedText += s;
						mode = prevMode;
					}
					else //Ouverture de la lecture de valeur
					{
						prevMode = mode;
						vstring = "";
						mode = ReadMode.VALUE;
					}
					break;
				case '@': //Lecture d'opération
					if (mode == ReadMode.OPERATION) //Fermeture de la lecture d'opération
					{
						if (operationType == OperationType.NONE) //Si aucune opération n'a été lue, on déclare juste la valeur
						{
							CreateValue(ostring);
						}
						else
						{
							Operation op = new Operation();
							op.key = ostring2;
							if (!float.TryParse(ostring, out value2)) //On essaie d'obtenir la valeur en chiffre du terme de droite
							{
								if (!GameManager.values.TryGetValue(ostring, out value2)) //Sinon, la valeur de la chaîne de charactère
								{

									Debug.Log("Erreur : clé inconnue " + ostring + " au caractère " + i);

								}
							}
							op.value = value2;
							op.operationType = operationType;
							if (readingDesc)
							{
								nextOperations[nextOperations.Count-1].Add(op);
							}
							else
							{
								ApplyOperation(op);
							}
							
						}
						
						mode = prevMode;
					}
					else // Ouverture de la lecture d'opération
					{
						prevMode = mode;
						ostring = "";
						mode = ReadMode.OPERATION;
						operationType = OperationType.NONE;
					}
					break;
				case '#'://Signale le début et la fin de la lecture de l'adresse d'un nextGameEvent
					if (mode == ReadMode.NEXT_ADRESS) //Fermeture
					{
						GameEvent ge = new GameEvent();
						ge.fileName = "Assets/Text/" + nstring + ".txt";
						nextGameEvents.Add(ge);

						readingDesc = true;
						nextOperations.Add(new HashSet<Operation>()); //On ajoute un set d'opérations (restera vide si aucune opération n'est associé à ce nextGameEvent)
						mode = ReadMode.TEXT; //Ensuite on va lire la description
					}
					else // Ouverture
					{
						nstring = "";
						mode = ReadMode.NEXT_ADRESS; 
					}
					break;
				case '\n'://Signale la fin de la lecture de description, on ajoute ensuite la desc qu'on a lu
					if (readingDesc)
					{
						nextDescriptions.Add(dstring);
						dstring= "";
						mode = ReadMode.TEXT;
						readingDesc = false;
					}
					else //Si on lit normalement
					{
						Debug.Log("Fin de ligne !");
						if (loadedText != "")//Si le texte est vide, on ne fait rien
						{
							nbOfReturn++;
							//Si c'est un simple saut de ligne, on ne fait rien
							if( nbOfReturn >= 2)//Si on saut 2 lignes, on fait une nouvelle boîte
							{
								GameObject b = GameObject.Instantiate(textBox);
								//On créé une boîte de texte
								b.transform.Find("Panel/Line").GetComponent<Text>().text = loadedText;
								boxes.Add(b);
								Debug.Log("Boîte ajoutée ! ");
								//Et on vide le buffer pour accueillir le texte de la prochaine boîte
								loadedText = "";
								nbOfReturn = 0;

							}
						}
					}
					break;
				case '='://Si mode Condition, alors condition égal
					if (mode == ReadMode.CONDITION)
					{
						condition = Condition.EQUALS;
						if (!GameManager.values.TryGetValue(cstring, out value1)) //On vérifie ce qu'il y a à gauche du '='
						{
							Debug.Log( "Erreur : clé inconnue " + cstring + " au caractère " + i);
						}
						cstring = "";
					}else if(mode == ReadMode.OPERATION) //Sinon, si on est en mode opération, c'est l'affectation
					{
						operationType = OperationType.ASSIGN;
						if (!GameManager.values.ContainsKey(ostring)) //On vérifie ce qu'il y a à gauche du '='
						{
							Debug.Log("Clé inconnue " + ostring + " au caractère " + i);
						}
						ostring2 = ostring; //On stocke ce qu'il a à gauche du '=' pour plus tard
						ostring = "";
					}
					else //sinon on affiche normalement
					{
						loadedText += c;
					}
					break;
				case '>'://Si mode condition, alors condition GREATER_THAN,
					if (mode == ReadMode.CONDITION)
					{
						condition = Condition.GREATER_THAN;
						if (!GameManager.values.TryGetValue(cstring, out value1))
						{
							Debug.Log( "Erreur : clé inconnue " + cstring + " au caractère " + i);
						}
						cstring = "";
					}
					else // sinon on affiche normalement
					{
						loadedText += c;
					}
					break;
				case '{': //Déjà géré avant le switch
					break;
				case '}': //idem
					break;
				default: //Pour tout autre caractère on met dans le buffer adapté
					if (c != 13) //Pour certaine raison, il y a un caractère (13) avant chaque saut de ligne
					{
						nbOfReturn = 0;
					}
					switch (mode)
					{
						case ReadMode.CONDITION:
							cstring += c;
							break;
						case ReadMode.VALUE:
							vstring += c;
							break;
						case ReadMode.NEXT_ADRESS:
							nstring += c;
							break;
						case ReadMode.OPERATION:
							ostring += c;
							break;
						default:
							if (readingDesc)
							{
								dstring += c;
							}
							else
							{
								loadedText += c;
							}
							break;
					}
					break;
			}
		}

		if(readingDesc && dstring != "") //Si on finit la lecture avec une description non comptée (ie oublié de sauter une ligne)
		{
			nextDescriptions.Add(dstring);

		}
	}

	static bool IsVerified(float value1, float value2, Condition c)
	{
		switch (c)
		{
			case Condition.EQUALS:
				return (value1 == value2);
			case Condition.GREATER_THAN:
				return (value1 > value2);
			default:
				Debug.Log("Erreur : Condition inconnue");
				return false;
		}
	}
	static void ApplyOperation(string key, float value, OperationType o)
	{
		if (GameManager.values.ContainsKey(key)) {
			switch (o)
			{
				case OperationType.ASSIGN:
					GameManager.values[key] = value;
					break;
				case OperationType.INCREMENT:
					GameManager.values[key] += value;
					break;
				default:
					Debug.Log("Erreur : Condition inconnue");
					break;
			}
		}
		else
		{
			if (o == OperationType.ASSIGN)
			{
				Debug.Log("Création de la clé " + key + " avec la valeur " + value);
				GameManager.values.Add(key, value);
			}
			else
			{
				Debug.Log("Erreur : clé inconnue " + key);
			}
		}
	}

	static void CreateValue(string key)
	{
		if (!GameManager.values.ContainsKey(key))
		{
			GameManager.values.Add(key, 0);
		}
	}

	static void ApplyOperation(string key, string value, OperationType o)
	{
		if (GameManager.names.ContainsKey(key))
		{
			switch (o)
			{
				case OperationType.ASSIGN:
					GameManager.names[key] = value;
					break;
				case OperationType.INCREMENT:
					GameManager.names[key] += value;
					break;
				default:
					Debug.Log("Erreur : Condition inconnue");
					break;
			}
		}
		else
		{
			if (o == OperationType.ASSIGN)
			{
				Debug.Log("Création de la clé " + key + " avec la valeur " + value);
				GameManager.names.Add(key, value);
			}
			else
			{
				Debug.Log("Erreur : clé inconnue " + key);
			}
		}
	}

	public static void ApplyOperation(Operation o)
	{
		ApplyOperation(o.key, o.value, o.operationType);
	}

	public GameObject GetNextBox()
	{
		GameObject box = boxes[currentBox];
		currentBox++;
		return box;
		
	}

}
