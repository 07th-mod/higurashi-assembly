using System;
using System.Collections;
using UnityEngine;

public class LeanTween : MonoBehaviour
{
	public static bool throwErrors = true;

	private static LTDescr[] tweens;

	private static int tweenMaxSearch = 0;

	private static int maxTweens = 400;

	private static int frameRendered = -1;

	private static GameObject _tweenEmpty;

	private static float dtEstimated;

	private static float previousRealTime;

	private static float dt;

	private static float dtActual;

	private static LTDescr tween;

	private static int i;

	private static int j;

	private static AnimationCurve punch = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1f, 0f));

	private static AnimationCurve shake = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f));

	private static Transform trans;

	private static float timeTotal;

	private static TweenAction tweenAction;

	private static float ratioPassed;

	private static float from;

	private static float to;

	private static float val;

	private static Vector3 newVect;

	private static bool isTweenFinished;

	private static GameObject target;

	private static GameObject customTarget;

	public static int startSearch = 0;

	public static LTDescr descr;

	private static Action<LTEvent>[] eventListeners;

	private static GameObject[] goListeners;

	private static int eventsMaxSearch = 0;

	public static int EVENTS_MAX = 10;

	public static int LISTENERS_MAX = 10;

	public static GameObject tweenEmpty
	{
		get
		{
			init(maxTweens);
			return _tweenEmpty;
		}
	}

	public static void init()
	{
		init(maxTweens);
	}

	public static void init(int maxSimultaneousTweens)
	{
		if (tweens == null)
		{
			maxTweens = maxSimultaneousTweens;
			tweens = new LTDescr[maxTweens];
			_tweenEmpty = new GameObject();
			_tweenEmpty.name = "~LeanTween";
			_tweenEmpty.AddComponent(typeof(LeanTween));
			_tweenEmpty.isStatic = true;
			_tweenEmpty.hideFlags = HideFlags.HideAndDontSave;
			UnityEngine.Object.DontDestroyOnLoad(_tweenEmpty);
			for (int i = 0; i < maxTweens; i++)
			{
				tweens[i] = new LTDescr();
			}
		}
	}

	public static void reset()
	{
		tweens = null;
	}

	public void Update()
	{
		update();
	}

	public void OnLevelWasLoaded(int lvl)
	{
		LTGUI.reset();
	}

	public static void update()
	{
		if (frameRendered != Time.frameCount)
		{
			init();
			dtEstimated = Time.realtimeSinceStartup - previousRealTime;
			if (dtEstimated > 0.2f)
			{
				dtEstimated = 0.2f;
			}
			previousRealTime = Time.realtimeSinceStartup;
			dtActual = Time.deltaTime * Time.timeScale;
			for (int i = 0; i < tweenMaxSearch && i < maxTweens; i++)
			{
				if (tweens[i].toggle)
				{
					tween = tweens[i];
					trans = tween.trans;
					timeTotal = tween.time;
					tweenAction = tween.type;
					dt = dtActual;
					if (tween.useEstimatedTime)
					{
						dt = dtEstimated;
						timeTotal = tween.time;
					}
					else if (tween.useFrames)
					{
						dt = 1f;
					}
					else if (tween.direction == 0f)
					{
						dt = 0f;
					}
					if (trans == null)
					{
						removeTween(i);
					}
					else
					{
						isTweenFinished = false;
						if (tween.delay <= 0f)
						{
							if (tween.passed + dt > timeTotal && tween.direction > 0f)
							{
								isTweenFinished = true;
								tween.passed = tween.time;
							}
							else if (tween.direction < 0f && tween.passed - dt < 0f)
							{
								isTweenFinished = true;
								tween.passed = Mathf.Epsilon;
							}
						}
						if (!tween.hasInitiliazed && (((double)tween.passed == 0.0 && (double)tween.delay == 0.0) || (double)tween.passed > 0.0))
						{
							tween.hasInitiliazed = true;
							if (!tween.useEstimatedTime)
							{
								tween.time *= Time.timeScale;
							}
							switch (tweenAction)
							{
							case TweenAction.MOVE:
								tween.from = trans.position;
								break;
							case TweenAction.MOVE_X:
							{
								ref Vector3 reference7 = ref tween.from;
								Vector3 position2 = trans.position;
								reference7.x = position2.x;
								break;
							}
							case TweenAction.MOVE_Y:
							{
								ref Vector3 reference8 = ref tween.from;
								Vector3 position3 = trans.position;
								reference8.x = position3.y;
								break;
							}
							case TweenAction.MOVE_Z:
							{
								ref Vector3 reference = ref tween.from;
								Vector3 position = trans.position;
								reference.x = position.z;
								break;
							}
							case TweenAction.MOVE_LOCAL_X:
							{
								ref Vector3 reference2 = ref tweens[i].from;
								Vector3 localPosition = trans.localPosition;
								reference2.x = localPosition.x;
								break;
							}
							case TweenAction.MOVE_LOCAL_Y:
							{
								ref Vector3 reference13 = ref tweens[i].from;
								Vector3 localPosition3 = trans.localPosition;
								reference13.x = localPosition3.y;
								break;
							}
							case TweenAction.MOVE_LOCAL_Z:
							{
								ref Vector3 reference12 = ref tweens[i].from;
								Vector3 localPosition2 = trans.localPosition;
								reference12.x = localPosition2.z;
								break;
							}
							case TweenAction.SCALE_X:
							{
								ref Vector3 reference11 = ref tween.from;
								Vector3 localScale3 = trans.localScale;
								reference11.x = localScale3.x;
								break;
							}
							case TweenAction.SCALE_Y:
							{
								ref Vector3 reference10 = ref tween.from;
								Vector3 localScale2 = trans.localScale;
								reference10.x = localScale2.y;
								break;
							}
							case TweenAction.SCALE_Z:
							{
								ref Vector3 reference9 = ref tween.from;
								Vector3 localScale = trans.localScale;
								reference9.x = localScale.z;
								break;
							}
							case TweenAction.ALPHA:
							{
								SpriteRenderer component = trans.gameObject.GetComponent<SpriteRenderer>();
								ref Vector3 reference6 = ref tween.from;
								float a;
								if (component != null)
								{
									Color color = component.color;
									a = color.a;
								}
								else
								{
									Color color2 = trans.gameObject.GetComponent<Renderer>().material.color;
									a = color2.a;
								}
								reference6.x = a;
								break;
							}
							case TweenAction.MOVE_LOCAL:
								tween.from = trans.localPosition;
								break;
							case TweenAction.MOVE_CURVED:
							case TweenAction.MOVE_CURVED_LOCAL:
							case TweenAction.MOVE_SPLINE:
							case TweenAction.MOVE_SPLINE_LOCAL:
								tween.from.x = 0f;
								break;
							case TweenAction.ROTATE:
								tween.from = trans.eulerAngles;
								tween.to = new Vector3(closestRot(tween.from.x, tween.to.x), closestRot(tween.from.y, tween.to.y), closestRot(tween.from.z, tween.to.z));
								break;
							case TweenAction.ROTATE_X:
							{
								ref Vector3 reference5 = ref tween.from;
								Vector3 eulerAngles3 = trans.eulerAngles;
								reference5.x = eulerAngles3.x;
								tween.to.x = closestRot(tween.from.x, tween.to.x);
								break;
							}
							case TweenAction.ROTATE_Y:
							{
								ref Vector3 reference4 = ref tween.from;
								Vector3 eulerAngles2 = trans.eulerAngles;
								reference4.x = eulerAngles2.y;
								tween.to.x = closestRot(tween.from.x, tween.to.x);
								break;
							}
							case TweenAction.ROTATE_Z:
							{
								ref Vector3 reference3 = ref tween.from;
								Vector3 eulerAngles = trans.eulerAngles;
								reference3.x = eulerAngles.z;
								tween.to.x = closestRot(tween.from.x, tween.to.x);
								break;
							}
							case TweenAction.ROTATE_AROUND:
								tween.lastVal = 0f;
								tween.origRotation = trans.eulerAngles;
								break;
							case TweenAction.ROTATE_LOCAL:
								tween.from = trans.localEulerAngles;
								tween.to = new Vector3(closestRot(tween.from.x, tween.to.x), closestRot(tween.from.y, tween.to.y), closestRot(tween.from.z, tween.to.z));
								break;
							case TweenAction.SCALE:
								tween.from = trans.localScale;
								break;
							case TweenAction.GUI_MOVE:
								tween.from = new Vector3(tween.ltRect.rect.x, tween.ltRect.rect.y, 0f);
								break;
							case TweenAction.GUI_MOVE_MARGIN:
								tween.from = new Vector2(tween.ltRect.margin.x, tween.ltRect.margin.y);
								break;
							case TweenAction.GUI_SCALE:
								tween.from = new Vector3(tween.ltRect.rect.width, tween.ltRect.rect.height, 0f);
								break;
							case TweenAction.GUI_ALPHA:
								tween.from.x = tween.ltRect.alpha;
								break;
							case TweenAction.GUI_ROTATE:
								if (!tween.ltRect.rotateEnabled)
								{
									tween.ltRect.rotateEnabled = true;
									tween.ltRect.resetForRotation();
								}
								tween.from.x = tween.ltRect.rotation;
								break;
							case TweenAction.ALPHA_VERTEX:
								tween.from.x = (float)(int)trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
								break;
							}
							tween.diff = tween.to - tween.from;
						}
						if (tween.delay <= 0f)
						{
							if (timeTotal <= 0f)
							{
								ratioPassed = 0f;
							}
							else
							{
								ratioPassed = tween.passed / timeTotal;
							}
							if (ratioPassed > 1f)
							{
								ratioPassed = 1f;
							}
							else if (ratioPassed < 0f)
							{
								ratioPassed = 0f;
							}
							if (tweenAction >= TweenAction.MOVE_X && tweenAction <= TweenAction.CALLBACK)
							{
								if (tween.animationCurve == null)
								{
									switch (tween.tweenType)
									{
									case LeanTweenType.linear:
										val = tween.from.x + tween.diff.x * ratioPassed;
										break;
									case LeanTweenType.easeOutQuad:
										val = easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
										break;
									case LeanTweenType.easeInQuad:
										val = easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutQuad:
										val = easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed);
										break;
									case LeanTweenType.easeInCubic:
										val = easeInCubic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutCubic:
										val = easeOutCubic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutCubic:
										val = easeInOutCubic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInQuart:
										val = easeInQuart(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutQuart:
										val = easeOutQuart(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutQuart:
										val = easeInOutQuart(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInQuint:
										val = easeInQuint(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutQuint:
										val = easeOutQuint(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutQuint:
										val = easeInOutQuint(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInSine:
										val = easeInSine(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutSine:
										val = easeOutSine(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutSine:
										val = easeInOutSine(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInExpo:
										val = easeInExpo(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutExpo:
										val = easeOutExpo(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutExpo:
										val = easeInOutExpo(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInCirc:
										val = easeInCirc(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutCirc:
										val = easeOutCirc(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutCirc:
										val = easeInOutCirc(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInBounce:
										val = easeInBounce(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutBounce:
										val = easeOutBounce(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutBounce:
										val = easeInOutBounce(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInBack:
										val = easeInBack(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutBack:
										val = easeOutBack(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutBack:
										val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInElastic:
										val = easeInElastic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeOutElastic:
										val = easeOutElastic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeInOutElastic:
										val = easeInOutElastic(tween.from.x, tween.to.x, ratioPassed);
										break;
									case LeanTweenType.easeShake:
									case LeanTweenType.punch:
										if (tween.tweenType == LeanTweenType.punch)
										{
											tween.animationCurve = punch;
										}
										else if (tween.tweenType == LeanTweenType.easeShake)
										{
											tween.animationCurve = shake;
										}
										tween.to.x = tween.from.x + tween.to.x;
										tween.diff.x = tween.to.x - tween.from.x;
										val = tweenOnCurve(tween, ratioPassed);
										break;
									case LeanTweenType.easeSpring:
										val = spring(tween.from.x, tween.to.x, ratioPassed);
										break;
									default:
										val = tween.from.x + tween.diff.x * ratioPassed;
										break;
									}
								}
								else
								{
									val = tweenOnCurve(tween, ratioPassed);
								}
								if (tweenAction == TweenAction.MOVE_X)
								{
									Transform transform = trans;
									float x = val;
									Vector3 position4 = trans.position;
									float y = position4.y;
									Vector3 position5 = trans.position;
									transform.position = new Vector3(x, y, position5.z);
								}
								else if (tweenAction == TweenAction.MOVE_Y)
								{
									Transform transform2 = trans;
									Vector3 position6 = trans.position;
									float x2 = position6.x;
									float y2 = val;
									Vector3 position7 = trans.position;
									transform2.position = new Vector3(x2, y2, position7.z);
								}
								else if (tweenAction == TweenAction.MOVE_Z)
								{
									Transform transform3 = trans;
									Vector3 position8 = trans.position;
									float x3 = position8.x;
									Vector3 position9 = trans.position;
									transform3.position = new Vector3(x3, position9.y, val);
								}
								if (tweenAction == TweenAction.MOVE_LOCAL_X)
								{
									Transform transform4 = trans;
									float x4 = val;
									Vector3 localPosition4 = trans.localPosition;
									float y3 = localPosition4.y;
									Vector3 localPosition5 = trans.localPosition;
									transform4.localPosition = new Vector3(x4, y3, localPosition5.z);
								}
								else if (tweenAction == TweenAction.MOVE_LOCAL_Y)
								{
									Transform transform5 = trans;
									Vector3 localPosition6 = trans.localPosition;
									float x5 = localPosition6.x;
									float y4 = val;
									Vector3 localPosition7 = trans.localPosition;
									transform5.localPosition = new Vector3(x5, y4, localPosition7.z);
								}
								else if (tweenAction == TweenAction.MOVE_LOCAL_Z)
								{
									Transform transform6 = trans;
									Vector3 localPosition8 = trans.localPosition;
									float x6 = localPosition8.x;
									Vector3 localPosition9 = trans.localPosition;
									transform6.localPosition = new Vector3(x6, localPosition9.y, val);
								}
								else if (tweenAction == TweenAction.MOVE_CURVED)
								{
									if (tween.path.orientToPath)
									{
										tween.path.place(trans, val);
									}
									else
									{
										trans.position = tween.path.point(val);
									}
								}
								else if (tweenAction == TweenAction.MOVE_CURVED_LOCAL)
								{
									if (tween.path.orientToPath)
									{
										tween.path.placeLocal(trans, val);
									}
									else
									{
										trans.localPosition = tween.path.point(val);
									}
								}
								else if (tweenAction == TweenAction.MOVE_SPLINE)
								{
									if (tween.spline.orientToPath)
									{
										tween.spline.place(trans, val);
									}
									else
									{
										trans.position = tween.spline.point(val);
									}
								}
								else if (tweenAction == TweenAction.MOVE_SPLINE_LOCAL)
								{
									if (tween.spline.orientToPath)
									{
										tween.spline.placeLocal(trans, val);
									}
									else
									{
										trans.localPosition = tween.spline.point(val);
									}
								}
								else if (tweenAction == TweenAction.SCALE_X)
								{
									Transform transform7 = trans;
									float x7 = val;
									Vector3 localScale4 = trans.localScale;
									float y5 = localScale4.y;
									Vector3 localScale5 = trans.localScale;
									transform7.localScale = new Vector3(x7, y5, localScale5.z);
								}
								else if (tweenAction == TweenAction.SCALE_Y)
								{
									Transform transform8 = trans;
									Vector3 localScale6 = trans.localScale;
									float x8 = localScale6.x;
									float y6 = val;
									Vector3 localScale7 = trans.localScale;
									transform8.localScale = new Vector3(x8, y6, localScale7.z);
								}
								else if (tweenAction == TweenAction.SCALE_Z)
								{
									Transform transform9 = trans;
									Vector3 localScale8 = trans.localScale;
									float x9 = localScale8.x;
									Vector3 localScale9 = trans.localScale;
									transform9.localScale = new Vector3(x9, localScale9.y, val);
								}
								else if (tweenAction == TweenAction.ROTATE_X)
								{
									Transform transform10 = trans;
									float x10 = val;
									Vector3 eulerAngles4 = trans.eulerAngles;
									float y7 = eulerAngles4.y;
									Vector3 eulerAngles5 = trans.eulerAngles;
									transform10.eulerAngles = new Vector3(x10, y7, eulerAngles5.z);
								}
								else if (tweenAction == TweenAction.ROTATE_Y)
								{
									Transform transform11 = trans;
									Vector3 eulerAngles6 = trans.eulerAngles;
									float x11 = eulerAngles6.x;
									float y8 = val;
									Vector3 eulerAngles7 = trans.eulerAngles;
									transform11.eulerAngles = new Vector3(x11, y8, eulerAngles7.z);
								}
								else if (tweenAction == TweenAction.ROTATE_Z)
								{
									Transform transform12 = trans;
									Vector3 eulerAngles8 = trans.eulerAngles;
									float x12 = eulerAngles8.x;
									Vector3 eulerAngles9 = trans.eulerAngles;
									transform12.eulerAngles = new Vector3(x12, eulerAngles9.y, val);
								}
								else if (tweenAction == TweenAction.ROTATE_AROUND)
								{
									float angle = val - tween.lastVal;
									if (isTweenFinished)
									{
										trans.eulerAngles = tween.origRotation;
										trans.RotateAround(trans.TransformPoint(tween.point), tween.axis, tween.to.x);
									}
									else
									{
										trans.RotateAround(trans.TransformPoint(tween.point), tween.axis, angle);
										tween.lastVal = val;
									}
								}
								else if (tweenAction == TweenAction.ALPHA)
								{
									SpriteRenderer component2 = trans.gameObject.GetComponent<SpriteRenderer>();
									if (component2 != null)
									{
										SpriteRenderer spriteRenderer = component2;
										Color color3 = component2.color;
										float r = color3.r;
										Color color4 = component2.color;
										float g = color4.g;
										Color color5 = component2.color;
										spriteRenderer.color = new Color(r, g, color5.b, val);
									}
									else
									{
										Material[] materials = trans.gameObject.GetComponent<Renderer>().materials;
										foreach (Material material in materials)
										{
											Material material2 = material;
											Color color6 = material.color;
											float r2 = color6.r;
											Color color7 = material.color;
											float g2 = color7.g;
											Color color8 = material.color;
											material2.color = new Color(r2, g2, color8.b, val);
										}
									}
								}
								else if (tweenAction == TweenAction.ALPHA_VERTEX)
								{
									Mesh mesh = trans.GetComponent<MeshFilter>().mesh;
									Vector3[] vertices = mesh.vertices;
									Color32[] array = new Color32[vertices.Length];
									Color32 color9 = mesh.colors32[0];
									color9 = new Color((float)(int)color9.r, (float)(int)color9.g, (float)(int)color9.b, val);
									for (int k = 0; k < vertices.Length; k++)
									{
										array[k] = color9;
									}
									mesh.colors32 = array;
								}
							}
							else if (tweenAction >= TweenAction.MOVE)
							{
								if (tween.animationCurve != null)
								{
									newVect = tweenOnCurveVector(tween, ratioPassed);
								}
								else if (tween.tweenType == LeanTweenType.linear)
								{
									newVect = new Vector3(tween.from.x + tween.diff.x * ratioPassed, tween.from.y + tween.diff.y * ratioPassed, tween.from.z + tween.diff.z * ratioPassed);
								}
								else if (tween.tweenType >= LeanTweenType.linear)
								{
									switch (tween.tweenType)
									{
									case LeanTweenType.easeOutQuad:
										newVect = new Vector3(easeOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
										break;
									case LeanTweenType.easeInQuad:
										newVect = new Vector3(easeInQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutQuad:
										newVect = new Vector3(easeInOutQuadOpt(tween.from.x, tween.diff.x, ratioPassed), easeInOutQuadOpt(tween.from.y, tween.diff.y, ratioPassed), easeInOutQuadOpt(tween.from.z, tween.diff.z, ratioPassed));
										break;
									case LeanTweenType.easeInCubic:
										newVect = new Vector3(easeInCubic(tween.from.x, tween.to.x, ratioPassed), easeInCubic(tween.from.y, tween.to.y, ratioPassed), easeInCubic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutCubic:
										newVect = new Vector3(easeOutCubic(tween.from.x, tween.to.x, ratioPassed), easeOutCubic(tween.from.y, tween.to.y, ratioPassed), easeOutCubic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutCubic:
										newVect = new Vector3(easeInOutCubic(tween.from.x, tween.to.x, ratioPassed), easeInOutCubic(tween.from.y, tween.to.y, ratioPassed), easeInOutCubic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInQuart:
										newVect = new Vector3(easeInQuart(tween.from.x, tween.to.x, ratioPassed), easeInQuart(tween.from.y, tween.to.y, ratioPassed), easeInQuart(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutQuart:
										newVect = new Vector3(easeOutQuart(tween.from.x, tween.to.x, ratioPassed), easeOutQuart(tween.from.y, tween.to.y, ratioPassed), easeOutQuart(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutQuart:
										newVect = new Vector3(easeInOutQuart(tween.from.x, tween.to.x, ratioPassed), easeInOutQuart(tween.from.y, tween.to.y, ratioPassed), easeInOutQuart(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInQuint:
										newVect = new Vector3(easeInQuint(tween.from.x, tween.to.x, ratioPassed), easeInQuint(tween.from.y, tween.to.y, ratioPassed), easeInQuint(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutQuint:
										newVect = new Vector3(easeOutQuint(tween.from.x, tween.to.x, ratioPassed), easeOutQuint(tween.from.y, tween.to.y, ratioPassed), easeOutQuint(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutQuint:
										newVect = new Vector3(easeInOutQuint(tween.from.x, tween.to.x, ratioPassed), easeInOutQuint(tween.from.y, tween.to.y, ratioPassed), easeInOutQuint(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInSine:
										newVect = new Vector3(easeInSine(tween.from.x, tween.to.x, ratioPassed), easeInSine(tween.from.y, tween.to.y, ratioPassed), easeInSine(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutSine:
										newVect = new Vector3(easeOutSine(tween.from.x, tween.to.x, ratioPassed), easeOutSine(tween.from.y, tween.to.y, ratioPassed), easeOutSine(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutSine:
										newVect = new Vector3(easeInOutSine(tween.from.x, tween.to.x, ratioPassed), easeInOutSine(tween.from.y, tween.to.y, ratioPassed), easeInOutSine(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInExpo:
										newVect = new Vector3(easeInExpo(tween.from.x, tween.to.x, ratioPassed), easeInExpo(tween.from.y, tween.to.y, ratioPassed), easeInExpo(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutExpo:
										newVect = new Vector3(easeOutExpo(tween.from.x, tween.to.x, ratioPassed), easeOutExpo(tween.from.y, tween.to.y, ratioPassed), easeOutExpo(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutExpo:
										newVect = new Vector3(easeInOutExpo(tween.from.x, tween.to.x, ratioPassed), easeInOutExpo(tween.from.y, tween.to.y, ratioPassed), easeInOutExpo(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInCirc:
										newVect = new Vector3(easeInCirc(tween.from.x, tween.to.x, ratioPassed), easeInCirc(tween.from.y, tween.to.y, ratioPassed), easeInCirc(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutCirc:
										newVect = new Vector3(easeOutCirc(tween.from.x, tween.to.x, ratioPassed), easeOutCirc(tween.from.y, tween.to.y, ratioPassed), easeOutCirc(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutCirc:
										newVect = new Vector3(easeInOutCirc(tween.from.x, tween.to.x, ratioPassed), easeInOutCirc(tween.from.y, tween.to.y, ratioPassed), easeInOutCirc(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInBounce:
										newVect = new Vector3(easeInBounce(tween.from.x, tween.to.x, ratioPassed), easeInBounce(tween.from.y, tween.to.y, ratioPassed), easeInBounce(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutBounce:
										newVect = new Vector3(easeOutBounce(tween.from.x, tween.to.x, ratioPassed), easeOutBounce(tween.from.y, tween.to.y, ratioPassed), easeOutBounce(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutBounce:
										newVect = new Vector3(easeInOutBounce(tween.from.x, tween.to.x, ratioPassed), easeInOutBounce(tween.from.y, tween.to.y, ratioPassed), easeInOutBounce(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInBack:
										newVect = new Vector3(easeInBack(tween.from.x, tween.to.x, ratioPassed), easeInBack(tween.from.y, tween.to.y, ratioPassed), easeInBack(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutBack:
										newVect = new Vector3(easeOutBack(tween.from.x, tween.to.x, ratioPassed), easeOutBack(tween.from.y, tween.to.y, ratioPassed), easeOutBack(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutBack:
										newVect = new Vector3(easeInOutBack(tween.from.x, tween.to.x, ratioPassed), easeInOutBack(tween.from.y, tween.to.y, ratioPassed), easeInOutBack(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInElastic:
										newVect = new Vector3(easeInElastic(tween.from.x, tween.to.x, ratioPassed), easeInElastic(tween.from.y, tween.to.y, ratioPassed), easeInElastic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeOutElastic:
										newVect = new Vector3(easeOutElastic(tween.from.x, tween.to.x, ratioPassed), easeOutElastic(tween.from.y, tween.to.y, ratioPassed), easeOutElastic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeInOutElastic:
										newVect = new Vector3(easeInOutElastic(tween.from.x, tween.to.x, ratioPassed), easeInOutElastic(tween.from.y, tween.to.y, ratioPassed), easeInOutElastic(tween.from.z, tween.to.z, ratioPassed));
										break;
									case LeanTweenType.easeShake:
									case LeanTweenType.punch:
										if (tween.tweenType == LeanTweenType.punch)
										{
											tween.animationCurve = punch;
										}
										else if (tween.tweenType == LeanTweenType.easeShake)
										{
											tween.animationCurve = shake;
										}
										tween.to = tween.from + tween.to;
										tween.diff = tween.to - tween.from;
										if (tweenAction == TweenAction.ROTATE || tweenAction == TweenAction.ROTATE_LOCAL)
										{
											tween.to = new Vector3(closestRot(tween.from.x, tween.to.x), closestRot(tween.from.y, tween.to.y), closestRot(tween.from.z, tween.to.z));
										}
										newVect = tweenOnCurveVector(tween, ratioPassed);
										break;
									case LeanTweenType.easeSpring:
										newVect = new Vector3(spring(tween.from.x, tween.to.x, ratioPassed), spring(tween.from.y, tween.to.y, ratioPassed), spring(tween.from.z, tween.to.z, ratioPassed));
										break;
									}
								}
								else
								{
									newVect = new Vector3(tween.from.x + tween.diff.x * ratioPassed, tween.from.y + tween.diff.y * ratioPassed, tween.from.z + tween.diff.z * ratioPassed);
								}
								if (tweenAction == TweenAction.MOVE)
								{
									trans.position = newVect;
								}
								else if (tweenAction == TweenAction.MOVE_LOCAL)
								{
									trans.localPosition = newVect;
								}
								else if (tweenAction == TweenAction.ROTATE)
								{
									trans.eulerAngles = newVect;
								}
								else if (tweenAction == TweenAction.ROTATE_LOCAL)
								{
									trans.localEulerAngles = newVect;
								}
								else if (tweenAction == TweenAction.SCALE)
								{
									trans.localScale = newVect;
								}
								else if (tweenAction == TweenAction.GUI_MOVE)
								{
									tween.ltRect.rect = new Rect(newVect.x, newVect.y, tween.ltRect.rect.width, tween.ltRect.rect.height);
								}
								else if (tweenAction == TweenAction.GUI_MOVE_MARGIN)
								{
									tween.ltRect.margin = new Vector2(newVect.x, newVect.y);
								}
								else if (tweenAction == TweenAction.GUI_SCALE)
								{
									tween.ltRect.rect = new Rect(tween.ltRect.rect.x, tween.ltRect.rect.y, newVect.x, newVect.y);
								}
								else if (tweenAction == TweenAction.GUI_ALPHA)
								{
									tween.ltRect.alpha = newVect.x;
								}
								else if (tweenAction == TweenAction.GUI_ROTATE)
								{
									tween.ltRect.rotation = newVect.x;
								}
							}
							if (tween.onUpdateFloat != null)
							{
								tween.onUpdateFloat(val);
							}
							else if (tween.onUpdateFloatObject != null)
							{
								tween.onUpdateFloatObject(val, tween.onUpdateParam);
							}
							else if (tween.onUpdateVector3Object != null)
							{
								tween.onUpdateVector3Object(newVect, tween.onUpdateParam);
							}
							else if (tween.onUpdateVector3 != null)
							{
								tween.onUpdateVector3(newVect);
							}
							else if (tween.optional != null)
							{
								object obj = tween.optional["onUpdate"];
								if (obj != null)
								{
									Hashtable arg = (Hashtable)tween.optional["onUpdateParam"];
									if (tweenAction == TweenAction.VALUE3)
									{
										if (obj.GetType() == typeof(string))
										{
											string methodName = obj as string;
											customTarget = ((tween.optional["onUpdateTarget"] == null) ? trans.gameObject : (tween.optional["onUpdateTarget"] as GameObject));
											customTarget.BroadcastMessage(methodName, newVect);
										}
										else if (obj.GetType() == typeof(Action<Vector3, Hashtable>))
										{
											Action<Vector3, Hashtable> action = (Action<Vector3, Hashtable>)obj;
											action(newVect, arg);
										}
										else
										{
											Action<Vector3> action2 = (Action<Vector3>)obj;
											action2(newVect);
										}
									}
									else if (obj.GetType() == typeof(string))
									{
										string methodName2 = obj as string;
										if (tween.optional["onUpdateTarget"] != null)
										{
											customTarget = (tween.optional["onUpdateTarget"] as GameObject);
											customTarget.BroadcastMessage(methodName2, val);
										}
										else
										{
											trans.gameObject.BroadcastMessage(methodName2, val);
										}
									}
									else if (obj.GetType() == typeof(Action<float, Hashtable>))
									{
										Action<float, Hashtable> action3 = (Action<float, Hashtable>)obj;
										action3(val, arg);
									}
									else if (obj.GetType() == typeof(Action<Vector3>))
									{
										Action<Vector3> action4 = (Action<Vector3>)obj;
										action4(newVect);
									}
									else
									{
										Action<float> action5 = (Action<float>)obj;
										action5(val);
									}
								}
							}
						}
						if (isTweenFinished)
						{
							if (tweenAction == TweenAction.GUI_ROTATE)
							{
								tween.ltRect.rotateFinished = true;
							}
							if (tween.loopType == LeanTweenType.once || tween.loopCount == 1)
							{
								if (tweenAction == TweenAction.DELAYED_SOUND)
								{
									AudioSource.PlayClipAtPoint((AudioClip)tween.onCompleteParam, tween.to, tween.from.x);
								}
								if (tween.onComplete != null)
								{
									removeTween(i);
									tween.onComplete();
								}
								else if (tween.onCompleteObject != null)
								{
									removeTween(i);
									tween.onCompleteObject(tween.onCompleteParam);
								}
								else if (tween.optional != null)
								{
									Action action6 = null;
									Action<object> action7 = null;
									string text = string.Empty;
									object obj2 = null;
									if (tween.optional != null && (bool)tween.trans && tween.optional["onComplete"] != null)
									{
										obj2 = tween.optional["onCompleteParam"];
										if (tween.optional["onComplete"].GetType() == typeof(string))
										{
											text = (tween.optional["onComplete"] as string);
										}
										else if (obj2 != null)
										{
											action7 = (Action<object>)tween.optional["onComplete"];
										}
										else
										{
											action6 = (Action)tween.optional["onComplete"];
											if (action6 == null)
											{
												Debug.LogWarning("callback was not converted");
											}
										}
									}
									removeTween(i);
									if (action7 != null)
									{
										action7(obj2);
									}
									else if (action6 != null)
									{
										action6();
									}
									else if (text != string.Empty)
									{
										if (tween.optional["onCompleteTarget"] != null)
										{
											customTarget = (tween.optional["onCompleteTarget"] as GameObject);
											if (obj2 != null)
											{
												customTarget.BroadcastMessage(text, obj2);
											}
											else
											{
												customTarget.BroadcastMessage(text);
											}
										}
										else if (obj2 != null)
										{
											trans.gameObject.BroadcastMessage(text, obj2);
										}
										else
										{
											trans.gameObject.BroadcastMessage(text);
										}
									}
								}
								else
								{
									removeTween(i);
								}
							}
							else
							{
								if (tween.loopCount < 0 && tween.type == TweenAction.CALLBACK)
								{
									if (tween.onComplete != null)
									{
										tween.onComplete();
									}
									else if (tween.onCompleteObject != null)
									{
										tween.onCompleteObject(tween.onCompleteParam);
									}
								}
								if (tween.loopCount >= 1)
								{
									tween.loopCount--;
								}
								if (tween.loopType == LeanTweenType.clamp)
								{
									tween.passed = Mathf.Epsilon;
								}
								else if (tween.loopType == LeanTweenType.pingPong)
								{
									tween.direction = 0f - tween.direction;
								}
							}
						}
						else if (tween.delay <= 0f)
						{
							tween.passed += dt * tween.direction;
						}
						else
						{
							tween.delay -= dt;
							if (tween.delay < 0f)
							{
								tween.passed = 0f;
								tween.delay = 0f;
							}
						}
					}
				}
			}
			frameRendered = Time.frameCount;
		}
	}

	public static void removeTween(int i)
	{
		if (tweens[i].toggle)
		{
			tweens[i].toggle = false;
			if (tweens[i].destroyOnComplete && tweens[i].ltRect != null)
			{
				LTGUI.destroy(tweens[i].ltRect.id);
			}
			startSearch = i;
			if (i + 1 >= tweenMaxSearch)
			{
				startSearch = 0;
				tweenMaxSearch--;
			}
		}
	}

	public static Vector3[] add(Vector3[] a, Vector3 b)
	{
		Vector3[] array = new Vector3[a.Length];
		for (i = 0; i < a.Length; i++)
		{
			array[i] = a[i] + b;
		}
		return array;
	}

	public static float closestRot(float from, float to)
	{
		float num = 0f - (360f - to);
		float num2 = 360f + to;
		float num3 = Mathf.Abs(to - from);
		float num4 = Mathf.Abs(num - from);
		float num5 = Mathf.Abs(num2 - from);
		if (num3 < num4 && num3 < num5)
		{
			return to;
		}
		if (num4 < num5)
		{
			return num;
		}
		return num2;
	}

	public static void cancel(GameObject gameObject)
	{
		init();
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				removeTween(i);
			}
		}
	}

	public static void cancel(GameObject gameObject, int uniqueId)
	{
		if (uniqueId >= 0)
		{
			init();
			int num = uniqueId & 0xFFFF;
			int num2 = uniqueId >> 16;
			if (tweens[num].trans == null || (tweens[num].trans.gameObject == gameObject && tweens[num].counter == num2))
			{
				removeTween(num);
			}
		}
	}

	public static void cancel(LTRect ltRect, int uniqueId)
	{
		if (uniqueId >= 0)
		{
			init();
			int num = uniqueId & 0xFFFF;
			int num2 = uniqueId >> 16;
			if (tweens[num].ltRect == ltRect && tweens[num].counter == num2)
			{
				removeTween(num);
			}
		}
	}

	private static void cancel(int uniqueId)
	{
		if (uniqueId >= 0)
		{
			init();
			int num = uniqueId & 0xFFFF;
			int num2 = uniqueId >> 16;
			if (tweens[num].hasInitiliazed && tweens[num].counter == num2)
			{
				removeTween(num);
			}
		}
	}

	public static LTDescr description(int uniqueId)
	{
		int num = uniqueId & 0xFFFF;
		int num2 = uniqueId >> 16;
		if (tweens[num] != null && tweens[num].uniqueId == uniqueId && tweens[num].counter == num2)
		{
			return tweens[num];
		}
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].uniqueId == uniqueId && tweens[i].counter == num2)
			{
				return tweens[i];
			}
		}
		return null;
	}

	public static void pause(GameObject gameObject, int uniqueId)
	{
		pause(uniqueId);
	}

	public static void pause(int uniqueId)
	{
		int num = uniqueId & 0xFFFF;
		int num2 = uniqueId >> 16;
		if (tweens[num].counter == num2)
		{
			tweens[num].pause();
		}
	}

	public static void pause(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				tweens[i].pause();
			}
		}
	}

	public static void resume(GameObject gameObject, int uniqueId)
	{
		resume(uniqueId);
	}

	public static void resume(int uniqueId)
	{
		int num = uniqueId & 0xFFFF;
		int num2 = uniqueId >> 16;
		if (tweens[num].counter == num2)
		{
			tweens[num].resume();
		}
	}

	public static void resume(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].trans == transform)
			{
				tweens[i].resume();
			}
		}
	}

	public static bool isTweening(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].toggle && tweens[i].trans == transform)
			{
				return true;
			}
		}
		return false;
	}

	public static bool isTweening(int uniqueId)
	{
		int num = uniqueId & 0xFFFF;
		int num2 = uniqueId >> 16;
		if (tweens[num].counter == num2 && tweens[num].toggle)
		{
			return true;
		}
		return false;
	}

	public static bool isTweening(LTRect ltRect)
	{
		for (int i = 0; i < tweenMaxSearch; i++)
		{
			if (tweens[i].toggle && tweens[i].ltRect == ltRect)
			{
				return true;
			}
		}
		return false;
	}

	public static void drawBezierPath(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		Vector3 vector = a;
		Vector3 a2 = -a + 3f * (b - c) + d;
		Vector3 b2 = 3f * (a + c) - 6f * b;
		Vector3 b3 = 3f * (b - a);
		for (float num = 1f; num <= 30f; num += 1f)
		{
			float d2 = num / 30f;
			Vector3 vector2 = ((a2 * d2 + b2) * d2 + b3) * d2 + a;
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}

	public static object logError(string error)
	{
		if (throwErrors)
		{
			Debug.LogError(error);
		}
		else
		{
			Debug.Log(error);
		}
		return null;
	}

	public static LTDescr options(LTDescr seed)
	{
		Debug.LogError("error this function is no longer used");
		return null;
	}

	public static LTDescr options()
	{
		init();
		j = 0;
		i = startSearch;
		while (j < maxTweens)
		{
			if (i >= maxTweens - 1)
			{
				i = 0;
			}
			if (!tweens[i].toggle)
			{
				if (i + 1 > tweenMaxSearch)
				{
					tweenMaxSearch = i + 1;
				}
				startSearch = i + 1;
				break;
			}
			j++;
			if (j >= maxTweens)
			{
				return logError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + maxTweens * 2 + " );") as LTDescr;
			}
			i++;
		}
		tween = tweens[i];
		tween.reset();
		tween.setId((uint)i);
		return tween;
	}

	private static LTDescr pushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, LTDescr tween)
	{
		init(maxTweens);
		if (gameObject == null)
		{
			return null;
		}
		tween.trans = gameObject.transform;
		tween.to = to;
		tween.time = time;
		tween.type = tweenAction;
		return tween;
	}

	public static LTDescr alpha(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA, options());
	}

	public static LTDescr alpha(LTRect ltRect, float to, float time)
	{
		ltRect.alphaEnabled = true;
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, options().setRect(ltRect));
	}

	public static LTDescr alphaVertex(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA_VERTEX, options());
	}

	public static LTDescr delayedCall(float delayTime, Action callback)
	{
		return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
	}

	public static LTDescr delayedCall(float delayTime, Action<object> callback)
	{
		return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
	}

	public static LTDescr delayedCall(GameObject gameObject, float delayTime, Action callback)
	{
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
	}

	public static LTDescr delayedCall(GameObject gameObject, float delayTime, Action<object> callback)
	{
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setOnComplete(callback));
	}

	public static LTDescr destroyAfter(LTRect rect, float delayTime)
	{
		return pushNewTween(tweenEmpty, Vector3.zero, delayTime, TweenAction.CALLBACK, options().setRect(rect).setDestroyOnComplete(doesDestroy: true));
	}

	public static LTDescr move(GameObject gameObject, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE, options());
	}

	public static LTDescr move(GameObject gameObject, Vector2 to, float time)
	{
		float x = to.x;
		float y = to.y;
		Vector3 position = gameObject.transform.position;
		return pushNewTween(gameObject, new Vector3(x, y, position.z), time, TweenAction.MOVE, options());
	}

	public static LTDescr move(GameObject gameObject, Vector3[] to, float time)
	{
		descr = options();
		if (descr.path == null)
		{
			descr.path = new LTBezierPath(to);
		}
		else
		{
			descr.path.setPoints(to);
		}
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED, descr);
	}

	public static LTDescr moveSpline(GameObject gameObject, Vector3[] to, float time)
	{
		descr = options();
		descr.spline = new LTSpline(to);
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_SPLINE, descr);
	}

	public static LTDescr moveSplineLocal(GameObject gameObject, Vector3[] to, float time)
	{
		descr = options();
		descr.spline = new LTSpline(to);
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_SPLINE_LOCAL, descr);
	}

	public static LTDescr move(LTRect ltRect, Vector2 to, float time)
	{
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_MOVE, options().setRect(ltRect));
	}

	public static LTDescr moveMargin(LTRect ltRect, Vector2 to, float time)
	{
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_MOVE_MARGIN, options().setRect(ltRect));
	}

	public static LTDescr moveX(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_X, options());
	}

	public static LTDescr moveY(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Y, options());
	}

	public static LTDescr moveZ(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Z, options());
	}

	public static LTDescr moveLocal(GameObject gameObject, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, options());
	}

	public static LTDescr moveLocal(GameObject gameObject, Vector3[] to, float time)
	{
		descr = options();
		if (descr.path == null)
		{
			descr.path = new LTBezierPath(to);
		}
		else
		{
			descr.path.setPoints(to);
		}
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED_LOCAL, descr);
	}

	public static LTDescr moveLocalX(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_X, options());
	}

	public static LTDescr moveLocalY(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Y, options());
	}

	public static LTDescr moveLocalZ(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Z, options());
	}

	public static LTDescr rotate(GameObject gameObject, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE, options());
	}

	public static LTDescr rotate(LTRect ltRect, float to, float time)
	{
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, options().setRect(ltRect));
	}

	public static LTDescr rotateLocal(GameObject gameObject, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, options());
	}

	public static LTDescr rotateX(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_X, options());
	}

	public static LTDescr rotateY(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Y, options());
	}

	public static LTDescr rotateZ(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Z, options());
	}

	public static LTDescr rotateAround(GameObject gameObject, Vector3 axis, float add, float time)
	{
		return pushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, options().setAxis(axis));
	}

	public static LTDescr scale(GameObject gameObject, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.SCALE, options());
	}

	public static LTDescr scale(LTRect ltRect, Vector2 to, float time)
	{
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_SCALE, options().setRect(ltRect));
	}

	public static LTDescr scaleX(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_X, options());
	}

	public static LTDescr scaleY(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Y, options());
	}

	public static LTDescr scaleZ(GameObject gameObject, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Z, options());
	}

	public static LTDescr value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, options().setTo(new Vector3(to, 0f, 0f)).setFrom(new Vector3(from, 0f, 0f)).setOnUpdate(callOnUpdate));
	}

	public static LTDescr value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time)
	{
		return pushNewTween(gameObject, to, time, TweenAction.VALUE3, options().setTo(to).setFrom(from).setOnUpdateVector3(callOnUpdate));
	}

	public static LTDescr value(GameObject gameObject, Action<float, object> callOnUpdate, float from, float to, float time)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, options().setTo(new Vector3(to, 0f, 0f)).setFrom(new Vector3(from, 0f, 0f)).setOnUpdateObject(callOnUpdate));
	}

	public static LTDescr delayedSound(AudioClip audio, Vector3 pos, float volume)
	{
		return pushNewTween(tweenEmpty, pos, 0f, TweenAction.DELAYED_SOUND, options().setTo(pos).setFrom(new Vector3(volume, 0f, 0f)).setAudio(audio));
	}

	public static Hashtable h(object[] arr)
	{
		if (arr.Length % 2 == 1)
		{
			logError("LeanTween - You have attempted to create a Hashtable with an odd number of values.");
			return null;
		}
		Hashtable hashtable = new Hashtable();
		for (i = 0; i < arr.Length; i += 2)
		{
			hashtable.Add(arr[i] as string, arr[i + 1]);
		}
		return hashtable;
	}

	private static int idFromUnique(int uniqueId)
	{
		return uniqueId & 0xFFFF;
	}

	private static int pushNewTween(GameObject gameObject, Vector3 to, float time, TweenAction tweenAction, Hashtable optional)
	{
		init(maxTweens);
		if (gameObject == null)
		{
			return -1;
		}
		j = 0;
		i = startSearch;
		while (j < maxTweens)
		{
			if (i >= maxTweens - 1)
			{
				i = 0;
			}
			if (!tweens[i].toggle)
			{
				if (i + 1 > tweenMaxSearch)
				{
					tweenMaxSearch = i + 1;
				}
				startSearch = i + 1;
				break;
			}
			j++;
			if (j >= maxTweens)
			{
				logError("LeanTween - You have run out of available spaces for tweening. To avoid this error increase the number of spaces to available for tweening when you initialize the LeanTween class ex: LeanTween.init( " + maxTweens * 2 + " );");
				return -1;
			}
			i++;
		}
		tween = tweens[i];
		tween.toggle = true;
		tween.reset();
		tween.trans = gameObject.transform;
		tween.to = to;
		tween.time = time;
		tween.type = tweenAction;
		tween.optional = optional;
		tween.setId((uint)i);
		if (optional != null)
		{
			object obj = optional["ease"];
			int num = 0;
			if (obj != null)
			{
				tween.tweenType = LeanTweenType.linear;
				if (obj.GetType() == typeof(LeanTweenType))
				{
					tween.tweenType = (LeanTweenType)(int)obj;
				}
				else if (obj.GetType() == typeof(AnimationCurve))
				{
					tween.animationCurve = (optional["ease"] as AnimationCurve);
				}
				else
				{
					string text = optional["ease"].ToString();
					if (text.Equals("easeOutQuad"))
					{
						tween.tweenType = LeanTweenType.easeOutQuad;
					}
					else if (text.Equals("easeInQuad"))
					{
						tween.tweenType = LeanTweenType.easeInQuad;
					}
					else if (text.Equals("easeInOutQuad"))
					{
						tween.tweenType = LeanTweenType.easeInOutQuad;
					}
				}
				num++;
			}
			if (optional["rect"] != null)
			{
				tween.ltRect = (LTRect)optional["rect"];
				num++;
			}
			if (optional["path"] != null)
			{
				tween.path = (LTBezierPath)optional["path"];
				num++;
			}
			if (optional["delay"] != null)
			{
				tween.delay = (float)optional["delay"];
				num++;
			}
			if (optional["useEstimatedTime"] != null)
			{
				tween.useEstimatedTime = (bool)optional["useEstimatedTime"];
				num++;
			}
			if (optional["useFrames"] != null)
			{
				tween.useFrames = (bool)optional["useFrames"];
				num++;
			}
			if (optional["loopType"] != null)
			{
				tween.loopType = (LeanTweenType)(int)optional["loopType"];
				num++;
			}
			if (optional["repeat"] != null)
			{
				tween.loopCount = (int)optional["repeat"];
				if (tween.loopType == LeanTweenType.once)
				{
					tween.loopType = LeanTweenType.clamp;
				}
				num++;
			}
			if (optional["point"] != null)
			{
				tween.point = (Vector3)optional["point"];
				num++;
			}
			if (optional["axis"] != null)
			{
				tween.axis = (Vector3)optional["axis"];
				num++;
			}
			if (optional.Count <= num)
			{
				tween.optional = null;
			}
		}
		else
		{
			tween.optional = null;
		}
		return tweens[i].uniqueId;
	}

	public static int value(string callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		return value(tweenEmpty, callOnUpdate, from, to, time, optional);
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time)
	{
		return value(gameObject, callOnUpdate, from, to, time, new Hashtable());
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, string callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional));
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, Action<float> callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional));
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, Action<float, Hashtable> callOnUpdate, float from, float to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.CALLBACK, optional));
		tweens[num].from = new Vector3(from, 0f, 0f);
		return num;
	}

	public static int value(GameObject gameObject, string callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, string callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onUpdate"] = callOnUpdate;
		int num = idFromUnique(pushNewTween(gameObject, to, time, TweenAction.VALUE3, optional));
		tweens[num].from = from;
		return num;
	}

	public static int value(GameObject gameObject, Action<Vector3> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int value(GameObject gameObject, Action<Vector3, Hashtable> callOnUpdate, Vector3 from, Vector3 to, float time, object[] optional)
	{
		return value(gameObject, callOnUpdate, from, to, time, h(optional));
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE, optional);
	}

	public static int rotate(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return rotate(gameObject, to, time, h(optional));
	}

	public static int rotate(LTRect ltRect, float to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ROTATE, optional);
	}

	public static int rotate(LTRect ltRect, float to, float time, object[] optional)
	{
		return rotate(ltRect, to, time, h(optional));
	}

	public static int rotateX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_X, optional);
	}

	public static int rotateX(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateX(gameObject, to, time, h(optional));
	}

	public static int rotateY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Y, optional);
	}

	public static int rotateY(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateY(gameObject, to, time, h(optional));
	}

	public static int rotateZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ROTATE_Z, optional);
	}

	public static int rotateZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return rotateZ(gameObject, to, time, h(optional));
	}

	public static int rotateLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.ROTATE_LOCAL, optional);
	}

	public static int rotateLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return rotateLocal(gameObject, to, time, h(optional));
	}

	public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["axis"] = axis;
		if (optional["point"] == null)
		{
			optional["point"] = Vector3.zero;
		}
		return pushNewTween(gameObject, new Vector3(add, 0f, 0f), time, TweenAction.ROTATE_AROUND, optional);
	}

	public static int rotateAround(GameObject gameObject, Vector3 axis, float add, float time, object[] optional)
	{
		return rotateAround(gameObject, axis, add, time, h(optional));
	}

	public static int moveX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_X, optional);
	}

	public static int moveX(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveX(gameObject, to, time, h(optional));
	}

	public static int moveY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Y, optional);
	}

	public static int moveY(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveY(gameObject, to, time, h(optional));
	}

	public static int moveZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_Z, optional);
	}

	public static int moveZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveZ(gameObject, to, time, h(optional));
	}

	public static int move(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE, optional);
	}

	public static int move(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return move(gameObject, to, time, h(optional));
	}

	public static int move(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
	{
		if (to.Length < 4)
		{
			string message = "LeanTween - When passing values for a vector path, you must pass four or more values!";
			if (throwErrors)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
			return -1;
		}
		if (to.Length % 4 != 0)
		{
			string message2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
			if (throwErrors)
			{
				Debug.LogError(message2);
			}
			else
			{
				Debug.Log(message2);
			}
			return -1;
		}
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		LTBezierPath lTBezierPath = new LTBezierPath(to);
		if (optional["orientToPath"] != null)
		{
			lTBezierPath.orientToPath = true;
		}
		optional["path"] = lTBezierPath;
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED, optional);
	}

	public static int move(GameObject gameObject, Vector3[] to, float time, object[] optional)
	{
		return move(gameObject, to, time, h(optional));
	}

	public static int move(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_MOVE, optional);
	}

	public static int move(LTRect ltRect, Vector3 to, float time, object[] optional)
	{
		return move(ltRect, to, time, h(optional));
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.MOVE_LOCAL, optional);
	}

	public static int moveLocal(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return moveLocal(gameObject, to, time, h(optional));
	}

	public static int moveLocal(GameObject gameObject, Vector3[] to, float time, Hashtable optional)
	{
		if (to.Length < 4)
		{
			string message = "LeanTween - When passing values for a vector path, you must pass four or more values!";
			if (throwErrors)
			{
				Debug.LogError(message);
			}
			else
			{
				Debug.Log(message);
			}
			return -1;
		}
		if (to.Length % 4 != 0)
		{
			string message2 = "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...";
			if (throwErrors)
			{
				Debug.LogError(message2);
			}
			else
			{
				Debug.Log(message2);
			}
			return -1;
		}
		init();
		if (optional == null)
		{
			optional = new Hashtable();
		}
		LTBezierPath lTBezierPath = new LTBezierPath(to);
		if (optional["orientToPath"] != null)
		{
			lTBezierPath.orientToPath = true;
		}
		optional["path"] = lTBezierPath;
		return pushNewTween(gameObject, new Vector3(1f, 0f, 0f), time, TweenAction.MOVE_CURVED_LOCAL, optional);
	}

	public static int moveLocal(GameObject gameObject, Vector3[] to, float time, object[] optional)
	{
		return moveLocal(gameObject, to, time, h(optional));
	}

	public static int moveLocalX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_X, optional);
	}

	public static int moveLocalX(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalX(gameObject, to, time, h(optional));
	}

	public static int moveLocalY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Y, optional);
	}

	public static int moveLocalY(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalY(gameObject, to, time, h(optional));
	}

	public static int moveLocalZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.MOVE_LOCAL_Z, optional);
	}

	public static int moveLocalZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return moveLocalZ(gameObject, to, time, h(optional));
	}

	public static int scale(GameObject gameObject, Vector3 to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, to, time, TweenAction.SCALE, optional);
	}

	public static int scale(GameObject gameObject, Vector3 to, float time, object[] optional)
	{
		return scale(gameObject, to, time, h(optional));
	}

	public static int scale(LTRect ltRect, Vector2 to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, to, time, TweenAction.GUI_SCALE, optional);
	}

	public static int scale(LTRect ltRect, Vector2 to, float time, object[] optional)
	{
		return scale(ltRect, to, time, h(optional));
	}

	public static int alpha(LTRect ltRect, float to, float time, Hashtable optional)
	{
		init();
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		ltRect.alphaEnabled = true;
		optional["rect"] = ltRect;
		return pushNewTween(tweenEmpty, new Vector3(to, 0f, 0f), time, TweenAction.GUI_ALPHA, optional);
	}

	public static int alpha(LTRect ltRect, float to, float time, object[] optional)
	{
		return alpha(ltRect, to, time, h(optional));
	}

	public static int scaleX(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_X, optional);
	}

	public static int scaleX(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleX(gameObject, to, time, h(optional));
	}

	public static int scaleY(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Y, optional);
	}

	public static int scaleY(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleY(gameObject, to, time, h(optional));
	}

	public static int scaleZ(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.SCALE_Z, optional);
	}

	public static int scaleZ(GameObject gameObject, float to, float time, object[] optional)
	{
		return scaleZ(gameObject, to, time, h(optional));
	}

	public static int delayedCall(float delayTime, string callback, Hashtable optional)
	{
		init();
		return delayedCall(tweenEmpty, delayTime, callback, optional);
	}

	public static int delayedCall(float delayTime, Action callback, object[] optional)
	{
		init();
		return delayedCall(tweenEmpty, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback, object[] optional)
	{
		return delayedCall(gameObject, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action callback, object[] optional)
	{
		return delayedCall(gameObject, delayTime, callback, h(optional));
	}

	public static int delayedCall(GameObject gameObject, float delayTime, string callback, Hashtable optional)
	{
		if (optional == null || optional.Count == 0)
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action callback, Hashtable optional)
	{
		if (optional == null)
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int delayedCall(GameObject gameObject, float delayTime, Action<object> callback, Hashtable optional)
	{
		if (optional == null)
		{
			optional = new Hashtable();
		}
		optional["onComplete"] = callback;
		return pushNewTween(gameObject, Vector3.zero, delayTime, TweenAction.CALLBACK, optional);
	}

	public static int alpha(GameObject gameObject, float to, float time, Hashtable optional)
	{
		return pushNewTween(gameObject, new Vector3(to, 0f, 0f), time, TweenAction.ALPHA, optional);
	}

	public static int alpha(GameObject gameObject, float to, float time, object[] optional)
	{
		return alpha(gameObject, to, time, h(optional));
	}

	private static float tweenOnCurve(LTDescr tweenDescr, float ratioPassed)
	{
		return tweenDescr.from.x + tweenDescr.diff.x * tweenDescr.animationCurve.Evaluate(ratioPassed);
	}

	private static Vector3 tweenOnCurveVector(LTDescr tweenDescr, float ratioPassed)
	{
		return new Vector3(tweenDescr.from.x + tweenDescr.diff.x * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.y + tweenDescr.diff.y * tweenDescr.animationCurve.Evaluate(ratioPassed), tweenDescr.from.z + tweenDescr.diff.z * tweenDescr.animationCurve.Evaluate(ratioPassed));
	}

	private static float easeOutQuadOpt(float start, float diff, float ratioPassed)
	{
		return (0f - diff) * ratioPassed * (ratioPassed - 2f) + start;
	}

	private static float easeInQuadOpt(float start, float diff, float ratioPassed)
	{
		return diff * ratioPassed * ratioPassed + start;
	}

	private static float easeInOutQuadOpt(float start, float diff, float ratioPassed)
	{
		ratioPassed /= 0.5f;
		if (ratioPassed < 1f)
		{
			return diff / 2f * ratioPassed * ratioPassed + start;
		}
		ratioPassed -= 1f;
		return (0f - diff) / 2f * (ratioPassed * (ratioPassed - 2f) - 1f) + start;
	}

	private static float linear(float start, float end, float val)
	{
		return Mathf.Lerp(start, end, val);
	}

	private static float clerp(float start, float end, float val)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float num4 = 0f;
		float num5 = 0f;
		if (end - start < 0f - num3)
		{
			num5 = (num2 - start + end) * val;
			return start + num5;
		}
		if (end - start > num3)
		{
			num5 = (0f - (num2 - end + start)) * val;
			return start + num5;
		}
		return start + (end - start) * val;
	}

	private static float spring(float start, float end, float val)
	{
		val = Mathf.Clamp01(val);
		val = (Mathf.Sin(val * 3.14159274f * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + 1.2f * (1f - val));
		return start + (end - start) * val;
	}

	private static float easeInQuad(float start, float end, float val)
	{
		end -= start;
		return end * val * val + start;
	}

	private static float easeOutQuad(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * val * (val - 2f) + start;
	}

	private static float easeInOutQuad(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val + start;
		}
		val -= 1f;
		return (0f - end) / 2f * (val * (val - 2f) - 1f) + start;
	}

	private static float easeInCubic(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val + start;
	}

	private static float easeOutCubic(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val + 1f) + start;
	}

	private static float easeInOutCubic(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val + start;
		}
		val -= 2f;
		return end / 2f * (val * val * val + 2f) + start;
	}

	private static float easeInQuart(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val + start;
	}

	private static float easeOutQuart(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return (0f - end) * (val * val * val * val - 1f) + start;
	}

	private static float easeInOutQuart(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val * val + start;
		}
		val -= 2f;
		return (0f - end) / 2f * (val * val * val * val - 2f) + start;
	}

	private static float easeInQuint(float start, float end, float val)
	{
		end -= start;
		return end * val * val * val * val * val + start;
	}

	private static float easeOutQuint(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * (val * val * val * val * val + 1f) + start;
	}

	private static float easeInOutQuint(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * val * val * val * val * val + start;
		}
		val -= 2f;
		return end / 2f * (val * val * val * val * val + 2f) + start;
	}

	private static float easeInSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * Mathf.Cos(val / 1f * 1.57079637f) + end + start;
	}

	private static float easeOutSine(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Sin(val / 1f * 1.57079637f) + start;
	}

	private static float easeInOutSine(float start, float end, float val)
	{
		end -= start;
		return (0f - end) / 2f * (Mathf.Cos(3.14159274f * val / 1f) - 1f) + start;
	}

	private static float easeInExpo(float start, float end, float val)
	{
		end -= start;
		return end * Mathf.Pow(2f, 10f * (val / 1f - 1f)) + start;
	}

	private static float easeOutExpo(float start, float end, float val)
	{
		end -= start;
		return end * (0f - Mathf.Pow(2f, -10f * val / 1f) + 1f) + start;
	}

	private static float easeInOutExpo(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return end / 2f * Mathf.Pow(2f, 10f * (val - 1f)) + start;
		}
		val -= 1f;
		return end / 2f * (0f - Mathf.Pow(2f, -10f * val) + 2f) + start;
	}

	private static float easeInCirc(float start, float end, float val)
	{
		end -= start;
		return (0f - end) * (Mathf.Sqrt(1f - val * val) - 1f) + start;
	}

	private static float easeOutCirc(float start, float end, float val)
	{
		val -= 1f;
		end -= start;
		return end * Mathf.Sqrt(1f - val * val) + start;
	}

	private static float easeInOutCirc(float start, float end, float val)
	{
		val /= 0.5f;
		end -= start;
		if (val < 1f)
		{
			return (0f - end) / 2f * (Mathf.Sqrt(1f - val * val) - 1f) + start;
		}
		val -= 2f;
		return end / 2f * (Mathf.Sqrt(1f - val * val) + 1f) + start;
	}

	private static float easeInBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		return end - easeOutBounce(0f, end, num - val) + start;
	}

	private static float easeOutBounce(float start, float end, float val)
	{
		val /= 1f;
		end -= start;
		if (val < 0.363636374f)
		{
			return end * (7.5625f * val * val) + start;
		}
		if (val < 0.727272749f)
		{
			val -= 0.545454562f;
			return end * (7.5625f * val * val + 0.75f) + start;
		}
		if ((double)val < 0.90909090909090906)
		{
			val -= 0.8181818f;
			return end * (7.5625f * val * val + 0.9375f) + start;
		}
		val -= 0.954545438f;
		return end * (7.5625f * val * val + 0.984375f) + start;
	}

	private static float easeInOutBounce(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		if (val < num / 2f)
		{
			return easeInBounce(0f, end, val * 2f) * 0.5f + start;
		}
		return easeOutBounce(0f, end, val * 2f - num) * 0.5f + end * 0.5f + start;
	}

	private static float easeInBack(float start, float end, float val)
	{
		end -= start;
		val /= 1f;
		float num = 1.70158f;
		return end * val * val * ((num + 1f) * val - num) + start;
	}

	private static float easeOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val = val / 1f - 1f;
		return end * (val * val * ((num + 1f) * val + num) + 1f) + start;
	}

	private static float easeInOutBack(float start, float end, float val)
	{
		float num = 1.70158f;
		end -= start;
		val /= 0.5f;
		if (val < 1f)
		{
			num *= 1.525f;
			return end / 2f * (val * val * ((num + 1f) * val - num)) + start;
		}
		val -= 2f;
		num *= 1.525f;
		return end / 2f * (val * val * ((num + 1f) * val + num) + 2f) + start;
	}

	private static float easeInElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num;
		if (val == 1f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
		}
		val -= 1f;
		return 0f - num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * 6.28318548f / num2) + start;
	}

	private static float easeOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num;
		if (val == 1f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
		}
		return num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * 6.28318548f / num2) + end + start;
	}

	private static float easeInOutElastic(float start, float end, float val)
	{
		end -= start;
		float num = 1f;
		float num2 = num * 0.3f;
		float num3 = 0f;
		float num4 = 0f;
		if (val == 0f)
		{
			return start;
		}
		val /= num / 2f;
		if (val == 2f)
		{
			return start + end;
		}
		if (num4 == 0f || num4 < Mathf.Abs(end))
		{
			num4 = end;
			num3 = num2 / 4f;
		}
		else
		{
			num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
		}
		if (val < 1f)
		{
			val -= 1f;
			return -0.5f * (num4 * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val * num - num3) * 6.28318548f / num2)) + start;
		}
		val -= 1f;
		return num4 * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * num - num3) * 6.28318548f / num2) * 0.5f + end + start;
	}

	public static void addListener(int eventId, Action<LTEvent> callback)
	{
		addListener(tweenEmpty, eventId, callback);
	}

	public static void addListener(GameObject caller, int eventId, Action<LTEvent> callback)
	{
		if (eventListeners == null)
		{
			eventListeners = new Action<LTEvent>[EVENTS_MAX * LISTENERS_MAX];
			goListeners = new GameObject[EVENTS_MAX * LISTENERS_MAX];
		}
		for (i = 0; i < LISTENERS_MAX; i++)
		{
			int num = eventId * LISTENERS_MAX + i;
			if (goListeners[num] == null || eventListeners[num] == null)
			{
				eventListeners[num] = callback;
				goListeners[num] = caller;
				if (i >= eventsMaxSearch)
				{
					eventsMaxSearch = i + 1;
				}
				return;
			}
			if (goListeners[num] == caller && object.ReferenceEquals(eventListeners[num], callback))
			{
				return;
			}
		}
		Debug.LogError("You ran out of areas to add listeners, consider increasing LISTENERS_MAX, ex: LeanTween.LISTENERS_MAX = " + LISTENERS_MAX * 2);
	}

	public static bool removeListener(int eventId, Action<LTEvent> callback)
	{
		return removeListener(tweenEmpty, eventId, callback);
	}

	public static bool removeListener(GameObject caller, int eventId, Action<LTEvent> callback)
	{
		for (i = 0; i < eventsMaxSearch; i++)
		{
			int num = eventId * LISTENERS_MAX + i;
			if (goListeners[num] == caller && object.ReferenceEquals(eventListeners[num], callback))
			{
				eventListeners[num] = null;
				goListeners[num] = null;
				return true;
			}
		}
		return false;
	}

	public static void dispatchEvent(int eventId)
	{
		dispatchEvent(eventId, null);
	}

	public static void dispatchEvent(int eventId, object data)
	{
		for (int i = 0; i < eventsMaxSearch; i++)
		{
			int num = eventId * LISTENERS_MAX + i;
			if (eventListeners[num] != null)
			{
				if ((bool)goListeners[num])
				{
					eventListeners[num](new LTEvent(eventId, data));
				}
				else
				{
					eventListeners[num] = null;
				}
			}
		}
	}
}
