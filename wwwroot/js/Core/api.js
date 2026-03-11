window.AppApi = {

  getCsrfToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  },

  async request(url, options = {}) {

    const opts = {
      credentials: 'same-origin',
      headers: {
        'Accept': 'application/json',
        ...(options.headers || {})
      },
      ...options
    };

    const res = await fetch(url, opts);

    if (!res.ok) {
      const txt = await res.text().catch(() => '');
      throw new Error(`HTTP ${res.status} :: ${txt}`);
    }

    const ct = res.headers.get("content-type");

    if (ct && ct.includes("application/json")) {
      return res.json();
    }

    return null;
  },

  async get(url) {
    return this.request(url);
  },

  async post(url, body) {
    return this.request(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': this.getCsrfToken()
      },
      body: JSON.stringify(body)
    });
  },

  async put(url, body) {
    return this.request(url, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': this.getCsrfToken()
      },
      body: JSON.stringify(body)
    });
  },

  async delete(url) {
    return this.request(url, {
      method: 'DELETE',
      headers: {
        'RequestVerificationToken': this.getCsrfToken()
      }
    });
  }

};