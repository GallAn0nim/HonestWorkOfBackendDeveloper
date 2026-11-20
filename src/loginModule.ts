import { api, ApiError, helpButtons, helpPopovers } from './engine';

const API_TIMEOUT_MS = 2000;
const NUDGE_FROM_FAILURE = 3;
const GROW_UNTIL_FAILURE = 5;
const OPEN_ABOUT_FROM_FAILURE = 6;
const BLINK_HINT_FROM_FAILURE = 7;
const RECOVER_AT_FAILURE = 8;
const NUDGE_SLOWEST_MS = 2000;
const NUDGE_SPEEDUP_MS = 250;
const GROW_STEP_EM = 0.2;

const markup = `
  <div class="mod-head">
    <span class="cell-only">05 / LOGIN</span><span class="open-only">05 / LOGIN</span>
    ${helpButtons('login', 'Login')}
  </div>
  <div class="signed-in cell-only" hidden>
    <span class="signed-label">SIGNED IN</span>
    <b class="cell-username"></b>
  </div>
  <svg class="lock cell-only" width="34" height="34" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><rect x="5" y="11" width="14" height="9" rx="2"/><path d="M8 11V8a4 4 0 0 1 8 0v3"/></svg>
  <div class="auth open-only">
    <form class="login-form">
      <label>Username<input name="username" autocomplete="username" autocapitalize="none" spellcheck="false" required></label>
      <label>Password
        <span class="secret">
          <input name="password" type="password" autocomplete="current-password" required>
          <button type="button" class="reveal" aria-label="Show password" title="Show password" aria-pressed="false">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/><circle cx="12" cy="12" r="3"/><path class="slash" d="m4 4 16 16"/></svg>
          </button>
        </span>
      </label>
      <button type="submit">Sign in</button>
      <button type="button" class="link forgot" data-view="recover">Forgot username or password?</button>
    </form>
    <form class="recover-form" hidden>
      <p class="recover-note" hidden>You clearly don't remember your credentials. Try to remind yourself of your username and password.</p>
      <label>Username<input name="username" autocomplete="off" autocapitalize="none" spellcheck="false" required></label>
      <div class="suggested" role="group" aria-labelledby="suggested-label">
        <span id="suggested-label">Suggested users</span>
        <div class="suggestions"></div>
      </div>
      <button type="submit">Remind password</button>
      <button type="button" class="link" data-view="login">Back to sign in</button>
    </form>
    <div class="profile" hidden>
      <dl>
        <dt>Username</dt><dd class="profile-username"></dd>
      </dl>
      <button type="button" class="sign-out">Sign out</button>
    </div>
  </div>
  <div class="mod-status"><span class="status" role="status"></span></div>
  ${helpPopovers('login', `
    <p>This hex is here to show a working login: sign in, password recovery and a real session. Signing in unlocks the other hexes.</p>
    <p>Go ahead and try any username and password. Failed attempts are expected and harmless, you are not breaking into anyone's account. Stuck? <span class="forgot-hint">"Forgot username or password?" suggests users to try.</span></p>`, `
    <p>Passwords are stored only as ASP.NET Core Identity hashes. An unknown username is still checked against a dummy hash, so a wrong user and a wrong password take the same time.</p>
    <p>A successful sign-in sets an HttpOnly, SameSite=Strict cookie, so the session survives a reload and is checked by GET /api/auth/me.</p>
    <p>Recovery ranks every username by Levenshtein edit distance to what you typed and suggests the closest three.</p>
    <p class="info-foot">Real users in PostgreSQL · real cookie session</p>`)}`;

interface SignedInUser {
  username: string;
}

export const createLoginModule = (onSessionChange: (signedIn: boolean) => void) => {
  const element = document.createElement('section');
  element.className = 'mod mod-login';
  element.setAttribute('aria-label', 'Login');
  element.innerHTML = markup;

  const loginForm = element.querySelector<HTMLFormElement>('.login-form')!;
  const recoverForm = element.querySelector<HTMLFormElement>('.recover-form')!;
  const password = loginForm.elements.namedItem('password') as HTMLInputElement;
  const reveal = element.querySelector<HTMLButtonElement>('.reveal')!;
  const profile = element.querySelector<HTMLElement>('.profile')!;
  const signedInCell = element.querySelector<HTMLElement>('.signed-in')!;
  const lock = element.querySelector('.lock')!;
  const suggestions = element.querySelector<HTMLElement>('.suggestions')!;
  const status = element.querySelector<HTMLElement>('.status')!;
  const forgot = element.querySelector<HTMLElement>('.forgot')!;
  const recoverNote = element.querySelector<HTMLElement>('.recover-note')!;
  const about = element.querySelector<HTMLElement>('#login-about')!;
  const forgotHint = about.querySelector('.forgot-hint')!;
  let failures = 0;

  function showView(view: string) {
    const signedIn = view === 'profile';
    loginForm.hidden = view !== 'login';
    recoverForm.hidden = view !== 'recover';
    profile.hidden = !signedIn;
    signedInCell.hidden = !signedIn;
    lock.toggleAttribute('hidden', signedIn);
    setStatus(signedIn ? '✓ SIGNED IN' : '', signedIn ? 'valid' : '');
    suggestions.replaceChildren();
    recoverNote.hidden = true;

    if (view !== 'login') {
      failures = 0;
      forgot.classList.remove('nudge');
      forgot.style.removeProperty('font-size');
      forgotHint.classList.remove('nudge');
    }
  }

  function countFailure() {
    failures++;

    if (failures >= RECOVER_AT_FAILURE) {
      about.hidePopover();
      showView('recover');
      recoverNote.hidden = false;
      return;
    }

    if (failures < NUDGE_FROM_FAILURE) {
      return;
    }

    element.style.setProperty('--nudge-ms', `${NUDGE_SLOWEST_MS - (failures - NUDGE_FROM_FAILURE) * NUDGE_SPEEDUP_MS}ms`);
    forgot.classList.add('nudge');
    forgot.style.fontSize = `${1 + (Math.min(failures, GROW_UNTIL_FAILURE) - NUDGE_FROM_FAILURE) * GROW_STEP_EM}em`;

    if (failures >= OPEN_ABOUT_FROM_FAILURE) {
      about.showPopover();
    }

    if (failures >= BLINK_HINT_FROM_FAILURE) {
      forgotHint.classList.add('nudge');
    }
  }

  function setStatus(text: string, result: string) {
    status.textContent = text;

    if (result) {
      element.dataset.result = result;
      return;
    }

    delete element.dataset.result;
  }

  function setPasswordVisible(visible: boolean) {
    password.type = visible ? 'text' : 'password';
    const label = visible ? 'Hide password' : 'Show password';
    reveal.setAttribute('aria-label', label);
    reveal.title = label;
    reveal.setAttribute('aria-pressed', String(visible));
  }

  async function post(form: HTMLFormElement, path: string, failure: string) {
    setStatus('SENDING…', '');
    suggestions.replaceChildren();

    try {
      return await api(path, AbortSignal.timeout(API_TIMEOUT_MS), Object.fromEntries(new FormData(form)));
    } catch (error) {
      setStatus(error instanceof ApiError && error.detail ? error.detail : failure, 'failed');
      return null;
    }
  }

  function showProfile(user: SignedInUser) {
    element.querySelector('.profile-username')!.textContent = user.username;
    element.querySelector('.cell-username')!.textContent = user.username;
    showView('profile');
    onSessionChange(true);
  }

  async function signOut() {
    try {
      await api('/api/auth/logout', AbortSignal.timeout(API_TIMEOUT_MS), {});
    } catch {
      setStatus('✕ SIGN OUT FAILED', 'failed');
      return;
    }

    showView('login');
    onSessionChange(false);
  }

  async function restoreSession() {
    try {
      showProfile(await api('/api/auth/me', AbortSignal.timeout(API_TIMEOUT_MS)));
    } catch (error) {
      if (!(error instanceof ApiError && error.status === 401)) {
        setStatus('✕ API UNREACHABLE', 'failed');
      }
    }
  }

  function suggestButton(username: string) {
    const button = document.createElement('button');
    button.type = 'button';
    button.textContent = username;
    button.addEventListener('click', () => {
      const recoverUsername = recoverForm.elements.namedItem('username') as HTMLInputElement;
      recoverUsername.value = username;
      recoverUsername.focus();
    });

    return button;
  }

  loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const response = await post(loginForm, '/api/auth/login', '✕ SIGN IN FAILED');

    if (!response) {
      countFailure();
      return;
    }

    loginForm.reset();
    showProfile(response);
  });
  recoverForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    const response = await post(recoverForm, '/api/auth/recover', '✕ REQUEST FAILED');

    if (!response) {
      return;
    }

    setStatus(response.message, response.suggestions ? '' : 'valid');
    suggestions.replaceChildren(...(response.suggestions ?? []).map(suggestButton));
  });
  element.querySelector('.sign-out')!.addEventListener('click', signOut);
  reveal.addEventListener('click', () => setPasswordVisible(password.type === 'password'));
  element.querySelectorAll<HTMLElement>('[data-view]').forEach((button) => {
    button.addEventListener('click', () => showView(button.dataset.view!));
  });

  restoreSession();

  return {
    element,
    setActive(active: boolean) {
      if (active) {
        return;
      }

      loginForm.reset();
      recoverForm.reset();
      setPasswordVisible(false);
      showView(profile.hidden ? 'login' : 'profile');
    },
  };
};
