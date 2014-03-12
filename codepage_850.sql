DROP FUNCTION IF EXISTS public.decode_cp850(text);
CREATE OR REPLACE FUNCTION public.decode_cp850(text)
RETURNS text AS
$BODY$
    SELECT CASE WHEN $1 ~ '[^\x01-\x7F]' THEN cp850_to_utf8($1) ELSE $1 END;        
$BODY$
LANGUAGE sql IMMUTABLE;

--

DROP FUNCTION IF EXISTS public.encode_cp850(text);
CREATE OR REPLACE FUNCTION public.encode_cp850(text)
RETURNS text AS
$BODY$
    SELECT CASE WHEN $1 ~ '[^\x01-\x7F]' THEN utf8_to_cp850($1) ELSE $1 END;        
$BODY$
LANGUAGE sql IMMUTABLE;
