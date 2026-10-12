const RequesterEmailDialog = ({ onConfirm, onCancel }) => {
  const handleSubmit = (event) => {
    event.preventDefault()
    const email = new FormData(event.currentTarget).get('RequesterEmail')
    onConfirm(String(email))
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4" role="presentation">
      <section
        role="dialog"
        aria-modal="true"
        aria-labelledby="requester-email-title"
        className="w-full max-w-md rounded-md bg-white p-6 shadow-xl"
      >
        <h2 id="requester-email-title" className="text-lg font-semibold text-gray-900">
          Your email address
        </h2>
        <p className="mt-1 text-sm text-gray-600">Enter an email address to associate with this request.</p>
        <form onSubmit={handleSubmit} className="mt-5">
          <label className="flex flex-col gap-1.5">
            <span className="font-medium text-gray-800">Email address</span>
            <input
              name="RequesterEmail"
              type="email"
              autoComplete="email"
              maxLength={254}
              required
              autoFocus
              className="w-full rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
            />
          </label>
          <div className="mt-6 flex justify-end gap-3">
            <button
              type="button"
              onClick={onCancel}
              className="rounded border border-gray-300 px-4 py-2 font-medium text-gray-800 hover:bg-gray-100"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="rounded bg-blue-800 px-4 py-2 font-semibold text-white hover:bg-blue-900"
            >
              Continue
            </button>
          </div>
        </form>
      </section>
    </div>
  )
}

export default RequesterEmailDialog