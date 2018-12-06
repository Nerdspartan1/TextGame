using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

enum ReadMode { TEXT, CONDITION, OPERATION, VALUE, NEXT_ADRESS, NEXT_DESC }
enum Condition { EQUALS, GREATER_THAN }
public enum OperationType { NONE, ASSIGN, ADD}

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

	public bool isMapLocation;
	
	public List<GameEvent> nextGameEvents = new List<GameEvent>();
	public List<string> nextDescriptions = new List<string>();
	public List<HashSet<Operation> > nextOperations = new List<HashSet<Operation> >();
	public List<GameObject> boxes = new List<GameObject>();

	public GameEvent(string eventName)
	{
		fileName = "Assets/Text/"+eventName+".txt";
	}

	public void Load()
	{

		string loadedText = "";

		string astring = ""; //adresse des prochains GameEvent
		string dstring = ""; //description des buttons
		string cstring = ""; //conditions
		string vstring = ""; //valeurs
		string ostring = ""; //opération
		string ostring2 = ""; //opération (buffer)
		string nstring = ""; //nom

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
		bool readingName = false;

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


			//Debug.Log(c);
			//Debug.Log((int)c);
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
							GameManager.CreateValue(ostring);
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
						GameEvent ge = new GameEvent(astring);
						nextGameEvents.Add(ge);

						readingDesc = true;
						nextOperations.Add(new HashSet<Operation>()); //On ajoute un set d'opérations (restera vide si aucune opération n'est associé à ce nextGameEvent)
						mode = ReadMode.TEXT; //Ensuite on va lire la description
					}
					else // Ouverture
					{
						astring = "";
						mode = ReadMode.NEXT_ADRESS; 
					}
					break;
				case '\n'://Signale la fin d'une boîte de texte ou signale la fin de la lecture de description, on ajoute ensuite la desc qu'on a lu
					if (readingDesc) //Si on lit une description
					{
						nextDescriptions.Add(dstring);
						dstring= "";
						mode = ReadMode.TEXT;
						readingDesc = false;
					}
					else //Si on lit normalement
					{
						//Debug.Log("Fin de ligne !");
						if (loadedText != "")//Si le texte est vide, on ne fait rien
						{
							nbOfReturn++;
							//Si c'est un simple saut de ligne, on ne fait rien
							if( nbOfReturn >= 2)//Si on saute 2 lignes, on fait une nouvelle boîte
							{
								GameObject b;
								if (nstring == "")
								{
									b=GameObject.Instantiate(textBox);
								}
								else
								{
									b = GameObject.Instantiate(dialogueBox);
									b.transform.Find("Panel/Name").GetComponent<Text>().text = nstring;
									b.transform.Find("Panel").GetComponent<Image>().color = GetColor(nstring);
									nstring = "";
								}
								//On créé une boîte de texte
								b.transform.Find("Panel/Line").GetComponent<Text>().text = loadedText;
								boxes.Add(b);
								//Debug.Log("Boîte ajoutée ! ");
								//Et on vide le buffer pour accueillir le texte de la prochaine boîte
								loadedText = "";
								nbOfReturn = 0;

							}
						}
					}
					break;
				case ':':
					if (readingName)
					{
						readingName = false;
					}
					else
					{
						loadedText += c;
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
					if (loadedText == "")//Si on est à un début de boîte
					{
						readingName = true;
					}
					else if (mode == ReadMode.CONDITION)
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
				case '+':
					if(mode == ReadMode.OPERATION)
					{
						operationType = OperationType.ADD;
						if (!GameManager.values.ContainsKey(ostring)) //On vérifie ce qu'il y a à gauche du '+'
						{
							Debug.Log("Clé inconnue " + ostring + " au caractère " + i);
						}
						ostring2 = ostring; //On stocke ce qu'il a à gauche du '+' pour plus tard
						ostring = "";
					}
					else
					{
						loadedText += c;
					}
					break;
				case '{': //Déjà géré avant le switch
					break;
				case '}': //idem
					break;
				default: //Pour tout autre caractère on met dans le buffer adapté
					if (c > 32) //Pour certaine raison, il peut y avoir un caractère spécial (de valeur <= 32) avant chaque saut de ligne. On ignore aussi les espace (32)
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
							astring += c;
							break;
						case ReadMode.OPERATION:
							ostring += c;
							break;
						default:
							if (readingDesc)
							{
								dstring += c;
							}
							else if (readingName)
							{
								nstring += c;
							}
							else
							{
								if (loadedText != "" || c > 32) //D'autre part, si le texte est vide, on ajoute pas les caractères invisibles
								{
									loadedText += c;
								}
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
				case OperationType.ADD:
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

	static void ApplyOperation(string key, string value, OperationType o)
	{
		if (GameManager.names.ContainsKey(key))
		{
			switch (o)
			{
				case OperationType.ASSIGN:
					GameManager.names[key] = value;
					break;
				case OperationType.ADD:
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
		
		//Renvoie la prochaine boîte (commençant par la 1ère)
		if(currentBox < boxes.Count)
		{
			GameObject box = boxes[currentBox];
			currentBox++;
			return box;
		}
		//Si on a atteint la fin de la liste, on renvoie null
		return null;
		
		
	}

	public bool EndOfGameEvent()
	{
		return (currentBox == boxes.Count);
	}

	public Color GetColor(string name)
	{
		float alpha = 0.5f;
		switch (name.Trim())
		{
			case "Deep voice":
			case "Boss":
				return new Color(0.5f, 0.5f, 0.5f, alpha);
			case "Sinister voice":
			case "Strike":
				return new Color(0.5f, 0.1f, 0.1f, alpha);
			case "Soft voice":
			case "Yau":
				return new Color(0.1f, 0.4f, 0.6f, alpha);
			case "Husky voice":
			case "Tank":
				return new Color(0.1f, 0.4f, 0.0f, alpha);
		}
		return new Color(0.6f, 0.6f, 0.6f, alpha);
	}

	public string IntToFullLetters(int n)
	{
		if (n == 0) return "zero";
		else if (n == 1) return "one";
		else if (n == 2) return "two";
		else if (n == 3) return "three";
		else if (n == 4) return "four";
		else if (n == 5) return "five";
		else if (n == 6) return "six";
		else if (n == 7) return "seven";
		else if (n == 8) return "eight";
		else if (n == 9) return "nine";
		else if (n == 10) return "ten";
		else if (n == 11) return "eleven";
		else if (n == 12) return "twelve";
		else if (n == 13) return "thirteen";
		else if (n == 14) return "fourteen";
		else if (n == 15) return "fifteen";
		else if (n == 16) return "sixteen";
		else if (n == 17) return "seventeen";
		else if (n == 18) return "eighteen";
		else if (n == 19) return "nineteen";
		else
		{
			int doz = n / 10;
			string s ="";
			if (doz == 2) s = "twenty";
			else if (doz == 3) s = "thirty";
			else if (doz == 4) s = "fourty";
			else if (doz == 5) s = "fifty";
			else if (doz == 6) s = "sixty";
			else if (doz == 7) s = "seventy";
			else if (doz == 8) s = "eighty";
			else if (doz == 9) s = "ninety";
			return s + (n - doz > 0 ? "-" + IntToFullLetters(n - doz) : "");
		}

	}

}
