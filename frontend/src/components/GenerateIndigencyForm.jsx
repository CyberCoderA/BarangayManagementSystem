const GenerateIndigencyForm = ({ onSubmit, isSubmitting, error, success }) => {
  const handleSubmit = (event) => {
    event.preventDefault()
    onSubmit(new FormData(event.currentTarget))
  }

  return (
    <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-4 sm:gap-5">
      <label className="flex flex-col gap-1.5">
        <span className="font-medium text-gray-800">Full name</span>
        <input
          name="FullName"
          type="text"
          required
          maxLength={150}
          placeholder="First Name M.I. Lastname"
          className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
        />
      </label>

      <label className="flex flex-col gap-1.5">
        <span className="font-medium text-gray-800">Home address</span>
        <input
          name="Address"
          type="text"
          required
          maxLength={250}
          className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
        />
      </label>

      <label className="flex flex-col gap-1.5">
        <span className="font-medium text-gray-800">Purpose</span>
        <input
          name="Purpose"
          type="text"
          required
          maxLength={250}
          className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
        />
      </label>

      <div className="flex flex-wrap items-center gap-4 pt-2">
        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full sm:w-auto rounded bg-blue-800 px-5 py-2.5 font-semibold text-white hover:bg-blue-900 disabled:cursor-wait disabled:opacity-60"
        >
          {isSubmitting ? 'Generating…' : 'Generate and download PDF'}
        </button>
        <p role="status" aria-live="polite" className="text-sm text-green-800">
          {success ? 'Certificate of indigency downloaded.' : ''}
        </p>
        {error && <p role="alert" className="text-sm text-red-700">{error}</p>}
      </div>
    </form>
  )
}

export default GenerateIndigencyForm