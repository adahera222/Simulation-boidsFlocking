Shader "NewShader" {
	
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Vishak
      inline half4 LightingVishak (SurfaceOutput s, half3 lightDir, half atten)
	{
		half diff = max (0, dot (s.Normal, lightDir));
		half factor = normalize (lightDir * s.Normal);
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * factor * 40);
		c.a = s.Alpha;
		return c;
	}


	inline half4 LightingVishak_PrePass (SurfaceOutput s, half4 light)
	{
		half4 c;
		c.rgb = s.Albedo * light.rgb;
		c.a = s.Alpha;
		return c;
	}
	
      struct Input {
          float4 color : COLOR;
      };
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = 1;
      }
      ENDCG
    }
    Fallback "Diffuse"
   }