#!/usr/bin/env bash
# Post-deploy smoke test for the live API + dashboard. Run after every merge
# to main and every Render/Vercel redeploy, before calling anything "working".
#
#   scripts/smoke-test.sh [API_URL] [SITE_URL]
#
# Uses curl only and writes no files. Exits non-zero if any check fails.
set -u

API="${1:-https://ig-ulsolutions-codingtask.onrender.com}"
SITE="${2:-https://dashboard-ten-fawn-97.vercel.app}"
API="${API%/}"; SITE="${SITE%/}"
FAILS=0

check() { # name expected actual
  if [ "$2" = "$3" ]; then printf '  PASS  %-42s %s\n' "$1" "$3"
  else printf '  FAIL  %-42s expected %s, got %s\n' "$1" "$2" "$3"; FAILS=$((FAILS + 1)); fi
}
code() { curl -s -m 90 -o /dev/null -w '%{http_code}' "$@"; }
json() { curl -s -m 30 "$@"; }

echo "API:  $API"
echo "SITE: $SITE"
echo

check "API /health (may take ~50s to wake)" 200 "$(code "$API/health")"
check "Evaluate without token" 401 "$(code -X POST "$API/api/expression/evaluate" -H 'Content-Type: application/json' -d '{"expression":"1+1"}')"

USER_NAME="smoke_$(date +%s)_$RANDOM"
BODY="{\"username\":\"$USER_NAME\",\"password\":\"SmokeTest12345\"}"
check "Register new user" 201 "$(code -X POST "$API/api/auth/register" -H 'Content-Type: application/json' -d "$BODY")"
check "Register duplicate user" 409 "$(code -X POST "$API/api/auth/register" -H 'Content-Type: application/json' -d "$BODY")"

TOKEN=$(json -X POST "$API/api/auth/login" -H 'Content-Type: application/json' -d "$BODY" | sed -n 's/.*"token":"\([^"]*\)".*/\1/p')
check "Login returns a token" yes "$([ -n "$TOKEN" ] && echo yes || echo no)"

for pair in '4+5*2=14' '4+5/2=6.5' '4+5/2-1=5.5'; do
  expr="${pair%=*}"; want="${pair##*=}"
  got=$(json -X POST "$API/api/expression/evaluate" -H 'Content-Type: application/json' -H "Authorization: Bearer $TOKEN" -d "{\"expression\":\"$expr\"}" | sed -n 's/.*"result":\([^,}]*\).*/\1/p')
  check "Evaluate $expr" "$want" "$got"
done
check "Division by zero rejected" 400 "$(code -X POST "$API/api/expression/evaluate" -H 'Content-Type: application/json' -H "Authorization: Bearer $TOKEN" -d '{"expression":"4+5/0"}')"

ACAO=$(curl -s -m 30 -o /dev/null -D - -X OPTIONS "$API/api/auth/login" -H "Origin: $SITE" -H 'Access-Control-Request-Method: POST' -H 'Access-Control-Request-Headers: content-type' | tr -d '\r' | sed -n 's/^[Aa]ccess-[Cc]ontrol-[Aa]llow-[Oo]rigin: //p')
check "CORS allows the site origin" "$SITE" "${ACAO:-none}"

check "Site is public (no Vercel login wall)" 200 "$(code "$SITE/")"
BUNDLE=$(curl -s -m 30 "$SITE/" | grep -o '/assets/index-[^"]*\.js' | head -1)
BAKED=$(curl -s -m 30 "$SITE$BUNDLE" | grep -o "https\?://${API#https://}/\?" | head -1)
check "Site is built against this API" "$API" "${BAKED%/}"

echo
if [ "$FAILS" -eq 0 ]; then echo "RESULT: PASS"; else echo "RESULT: FAIL ($FAILS check(s))"; exit 1; fi
