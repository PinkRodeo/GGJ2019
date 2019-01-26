// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/DiffuseWithDepth"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		_BlurAmount ("Blur Amount", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

        struct Input	
        {
            float2 uv_MainTex;
            fixed4 color;
			float blurAmount;
        };

		float _BlurAmount;

        void vert (inout appdata_full v, out Input o)
        {
            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color * _RendererColor;

			o.blurAmount = _BlurAmount*0.1;
        }
		//https://answers.unity.com/questions/407214/gaussian-blur-shader.html
		//normpdf function gives us a Guassian distribution for each blur iteration; 
		//this is equivalent of multiplying by hard #s 0.16,0.15,0.12,0.09, etc. in code above
		float normpdf(float x, float sigma)
		{
			return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
		}
		//this is the blur function... pass in standard col derived from tex2d(_MainTex,i.uv)
		half4 blur(sampler2D tex, float2 uv,float blurAmount, float4 tintColor) {
			//get our base color...
			half4 col = tex2D(tex, uv) * tintColor;
			//total width/height of our blur "grid":
			const int mSize = 11;
			//this gives the number of times we'll iterate our blur on each side 
			//(up,down,left,right) of our uv coordinate;
			//NOTE that this needs to be a const or you'll get errors about unrolling for loops
			const int iter = (mSize - 1) / 2;
			//run loops to do the equivalent of what's written out line by line above
			//(number of blur iterations can be easily sized up and down this way)
			for (int i = -iter; i <= iter; ++i) {
				for (int j = -iter; j <= iter; ++j) {
					col += tex2D(tex, float2(uv.x + i * blurAmount, uv.y + j * blurAmount)) * tintColor * normpdf(float(i), 3); // this 3 used to be 7 in the example code
				}
			}
			//return blurred color
			return col/mSize;
		 }

        void surf (Input IN, inout SurfaceOutput o)
        {
		    //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
            fixed4 c = blur(_MainTex, IN.uv_MainTex, IN.blurAmount, IN.color);

			//SampleSpriteTexture (IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;
        }



        ENDCG
    }

	Fallback "Transparent/VertexLit"
}
